using Notification.Api.DTOs;

namespace Notification.Api.Services;

public interface ICampaignService
{
    Task<CampaignDetailResponse> TaoChienDichAsync(
        CreateCampaignRequest yeuCau,
        CancellationToken tokenHuy);

    Task<IReadOnlyList<CampaignResponse>> LayDanhSachChienDichAsync(CancellationToken tokenHuy);

    Task<CampaignDetailResponse?> LayChiTietChienDichAsync(int ma, CancellationToken tokenHuy);

    Task<CampaignStartResult> BatDauChienDichAsync(int ma, CancellationToken tokenHuy);
}
