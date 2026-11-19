using System.ComponentModel.DataAnnotations;

namespace Notification.Api.DTOs;

public class CreateCampaignRequest : IValidatableObject
{
    [Required(ErrorMessage = "Ten chien dich la bat buoc.")]
    [StringLength(200, ErrorMessage = "Ten chien dich khong duoc vuot qua 200 ky tu.")]
    public string TenChienDich { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tieu de la bat buoc.")]
    [StringLength(255, ErrorMessage = "Tieu de khong duoc vuot qua 255 ky tu.")]
    public string TieuDe { get; set; } = string.Empty;

    [Required(ErrorMessage = "Noi dung la bat buoc.")]
    public string NoiDung { get; set; } = string.Empty;

    [Required(ErrorMessage = "Danh sach nguoi nhan la bat buoc.")]
    [MinLength(1, ErrorMessage = "Danh sach nguoi nhan phai co it nhat mot nguoi.")]
    public List<RecipientRequest> DanhSachNguoiNhan { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext nguCanh)
    {
        if (DanhSachNguoiNhan is null)
        {
            yield break;
        }

        if (DanhSachNguoiNhan.Any(nguoiNhan => nguoiNhan is null))
        {
            yield return new ValidationResult(
                "Moi phan tu trong danh sach nguoi nhan phai co thong tin.",
                [nameof(DanhSachNguoiNhan)]);
            yield break;
        }

        var emailBiTrung = DanhSachNguoiNhan
            .Where(nguoiNhan => !string.IsNullOrWhiteSpace(nguoiNhan.DiaChiEmail))
            .GroupBy(nguoiNhan => nguoiNhan.DiaChiEmail.Trim(), StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(nhom => nhom.Count() > 1);

        if (emailBiTrung is not null)
        {
            yield return new ValidationResult(
                "Danh sach nguoi nhan khong duoc co dia chi email trung lap.",
                [nameof(DanhSachNguoiNhan)]);
        }
    }
}
