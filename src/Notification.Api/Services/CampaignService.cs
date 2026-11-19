using Microsoft.EntityFrameworkCore;
using Notification.Api.DTOs;
using Notification.Core.Entities;
using Notification.Core.Enums;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Messaging;

namespace Notification.Api.Services;

public class CampaignService(
    ApplicationDbContext coSoDuLieu,
    INotificationPublisher notificationPublisher,
    ILogger<CampaignService> logger) : ICampaignService
{
    public async Task<CampaignDetailResponse> TaoChienDichAsync(
        CreateCampaignRequest yeuCau,
        CancellationToken tokenHuy)
    {
        var chienDich = new Campaign
        {
            TenChienDich = yeuCau.TenChienDich.Trim(),
            TieuDe = yeuCau.TieuDe.Trim(),
            NoiDung = yeuCau.NoiDung.Trim(),
            TrangThai = CampaignStatus.Draft,
            Recipients = yeuCau.DanhSachNguoiNhan
                .Select(nguoiNhan => new Recipient
                {
                    HoTen = nguoiNhan.HoTen.Trim(),
                    DiaChiEmail = nguoiNhan.DiaChiEmail.Trim()
                })
                .ToList()
        };

        coSoDuLieu.Campaigns.Add(chienDich);
        await coSoDuLieu.SaveChangesAsync(tokenHuy);

        return ChuyenSangChiTiet(chienDich);
    }

    public async Task<IReadOnlyList<CampaignResponse>> LayDanhSachChienDichAsync(
        CancellationToken tokenHuy)
    {
        return await coSoDuLieu.Campaigns
            .AsNoTracking()
            .OrderByDescending(chienDich => chienDich.NgayTao)
            .ThenByDescending(chienDich => chienDich.Ma)
            .Select(chienDich => new CampaignResponse
            {
                Ma = chienDich.Ma,
                TenChienDich = chienDich.TenChienDich,
                TieuDe = chienDich.TieuDe,
                TrangThai = chienDich.TrangThai.ToString(),
                NgayTao = chienDich.NgayTao,
                SoLuongNguoiNhan = chienDich.Recipients.Count
            })
            .ToListAsync(tokenHuy);
    }

    public async Task<CampaignDetailResponse?> LayChiTietChienDichAsync(
        int ma,
        CancellationToken tokenHuy)
    {
        var chienDich = await coSoDuLieu.Campaigns
            .AsNoTracking()
            .Include(chienDich => chienDich.Recipients)
            .SingleOrDefaultAsync(chienDich => chienDich.Ma == ma, tokenHuy);

        return chienDich is null ? null : ChuyenSangChiTiet(chienDich);
    }

    public async Task<CampaignStartResult> BatDauChienDichAsync(int ma, CancellationToken tokenHuy)
    {
        var chienDich = await coSoDuLieu.Campaigns
            .Include(chienDich => chienDich.Recipients)
            .Include(chienDich => chienDich.Deliveries)
            .SingleOrDefaultAsync(chienDich => chienDich.Ma == ma, tokenHuy);

        if (chienDich is null)
        {
            return TaoKetQuaLoi(CampaignStartFailure.NotFound, "Khong tim thay chien dich.");
        }

        if (chienDich.Recipients.Count == 0)
        {
            return TaoKetQuaLoi(
                CampaignStartFailure.NoRecipients,
                "Chien dich phai co it nhat mot nguoi nhan.");
        }

        if (chienDich.TrangThai != CampaignStatus.Draft)
        {
            return TaoKetQuaLoi(
                CampaignStartFailure.AlreadyStarted,
                "Chi co the bat dau chien dich o trang thai Draft.");
        }

        var daNhanQuyenXuLy = await coSoDuLieu.Campaigns
            .Where(
                muc => muc.Ma == ma &&
                       muc.TrangThai == CampaignStatus.Draft)
            .ExecuteUpdateAsync(
                capNhat => capNhat.SetProperty(
                    muc => muc.TrangThai,
                    CampaignStatus.Processing),
                tokenHuy);

        if (daNhanQuyenXuLy == 0)
        {
            return TaoKetQuaLoi(
                CampaignStartFailure.AlreadyStarted,
                "Chien dich dang duoc bat dau boi mot yeu cau khac hoac da duoc bat dau.");
        }

        chienDich.TrangThai = CampaignStatus.Processing;
        chienDich.NgayBatDau ??= DateTimeOffset.UtcNow;

        var nguoiNhanDaCoDelivery = chienDich.Deliveries
            .Select(lanGui => lanGui.MaNguoiNhan)
            .ToHashSet();
        var deliveriesMoi = chienDich.Recipients
            .Where(nguoiNhan => !nguoiNhanDaCoDelivery.Contains(nguoiNhan.Ma))
            .Select(nguoiNhan => new Delivery
            {
                MaChienDich = chienDich.Ma,
                MaNguoiNhan = nguoiNhan.Ma,
                Campaign = chienDich,
                Recipient = nguoiNhan,
                TrangThai = DeliveryStatus.Pending,
                SoLanThuLai = 0,
                NgayTao = DateTimeOffset.UtcNow
            })
            .ToList();

        coSoDuLieu.Deliveries.AddRange(deliveriesMoi);

        try
        {
            await coSoDuLieu.SaveChangesAsync(tokenHuy);
        }
        catch
        {
            await DatLaiTrangThaiNeuCoTheAsync(chienDich);
            throw;
        }

        foreach (var lanGui in chienDich.Deliveries.Where(lanGui => !lanGui.DaDuaVaoHangDoi))
        {
            try
            {
                await notificationPublisher.PublishAsync(
                    new NotificationMessage(lanGui.Ma, chienDich.Ma),
                    tokenHuy);
            }
            catch (OperationCanceledException) when (tokenHuy.IsCancellationRequested)
            {
                await DatLaiTrangThaiNeuCoTheAsync(chienDich);
                throw;
            }
            catch (Exception loi)
            {
                logger.LogError(
                    loi,
                    "Khong the dua Delivery {DeliveryId} cua Campaign {CampaignId} vao RabbitMQ.",
                    lanGui.Ma,
                    chienDich.Ma);

                lanGui.ThongBaoLoiDayVaoHangDoi =
                    "Khong the publish message vao RabbitMQ. Hay kiem tra broker va thu lai chien dich.";
                chienDich.TrangThai = CampaignStatus.Draft;
                await coSoDuLieu.SaveChangesAsync(CancellationToken.None);

                return TaoKetQuaHangDoi(chienDich, deliveriesMoi.Count, CampaignStartFailure.QueueUnavailable);
            }

            lanGui.DaDuaVaoHangDoi = true;
            lanGui.ThongBaoLoiDayVaoHangDoi = null;
            await coSoDuLieu.SaveChangesAsync(tokenHuy);
        }

        chienDich.TrangThai = CampaignStatus.Queued;
        await coSoDuLieu.SaveChangesAsync(tokenHuy);

        return TaoKetQuaHangDoi(chienDich, deliveriesMoi.Count, CampaignStartFailure.None);
    }

    private static CampaignStartResult TaoKetQuaLoi(CampaignStartFailure loi, string thongBao)
    {
        return new CampaignStartResult(
            loi,
            new StartCampaignResponse { ThongBao = thongBao });
    }

    private static CampaignStartResult TaoKetQuaHangDoi(
        Campaign chienDich,
        int soDeliveryMoi,
        CampaignStartFailure loi)
    {
        var soMessageDaDuaVaoQueue = chienDich.Deliveries.Count(lanGui => lanGui.DaDuaVaoHangDoi);
        var soMessageChuaDuaVaoQueue = chienDich.Deliveries.Count - soMessageDaDuaVaoQueue;
        var thongBao = loi == CampaignStartFailure.None
            ? "Da dua tat ca cong viec vao hang doi. Chua co email nao duoc gui."
            : $"Da dua {soMessageDaDuaVaoQueue} tren {chienDich.Deliveries.Count} cong viec vao hang doi. " +
              $"{soMessageChuaDuaVaoQueue} cong viec chua publish; co the bat dau lai sau khi RabbitMQ hoat dong.";

        return new CampaignStartResult(
            loi,
            new StartCampaignResponse
            {
                MaChienDich = chienDich.Ma,
                TenChienDich = chienDich.TenChienDich,
                TongSoNguoiNhan = chienDich.Recipients.Count,
                SoDeliveryDuocTao = soDeliveryMoi,
                SoMessageDaDuaVaoQueue = soMessageDaDuaVaoQueue,
                SoMessageChuaDuaVaoQueue = soMessageChuaDuaVaoQueue,
                TrangThai = chienDich.TrangThai.ToString(),
                ThongBao = thongBao
            });
    }

    private async Task DatLaiTrangThaiNeuCoTheAsync(Campaign chienDich)
    {
        try
        {
            chienDich.TrangThai = CampaignStatus.Draft;
            await coSoDuLieu.SaveChangesAsync(CancellationToken.None);
        }
        catch (Exception loi)
        {
            logger.LogError(
                loi,
                "Khong the dat lai Campaign {CampaignId} ve Draft sau loi khi bat dau.",
                chienDich.Ma);
        }
    }

    private static CampaignDetailResponse ChuyenSangChiTiet(Campaign chienDich)
    {
        return new CampaignDetailResponse
        {
            Ma = chienDich.Ma,
            TenChienDich = chienDich.TenChienDich,
            TieuDe = chienDich.TieuDe,
            NoiDung = chienDich.NoiDung,
            TrangThai = chienDich.TrangThai.ToString(),
            NgayTao = chienDich.NgayTao,
            NgayBatDau = chienDich.NgayBatDau,
            NgayHoanThanh = chienDich.NgayHoanThanh,
            SoLuongNguoiNhan = chienDich.Recipients.Count,
            NguoiNhans = chienDich.Recipients
                .Select(nguoiNhan => new RecipientResponse
                {
                    Ma = nguoiNhan.Ma,
                    HoTen = nguoiNhan.HoTen,
                    DiaChiEmail = nguoiNhan.DiaChiEmail,
                    NgayTao = nguoiNhan.NgayTao
                })
                .ToList()
        };
    }
}
