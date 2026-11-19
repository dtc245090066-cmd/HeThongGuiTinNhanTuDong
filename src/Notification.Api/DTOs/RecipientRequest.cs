using System.ComponentModel.DataAnnotations;

namespace Notification.Api.DTOs;

public class RecipientRequest
{
    [Required(ErrorMessage = "Ho ten nguoi nhan la bat buoc.")]
    [StringLength(200, ErrorMessage = "Ho ten nguoi nhan khong duoc vuot qua 200 ky tu.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dia chi email la bat buoc.")]
    [EmailAddress(ErrorMessage = "Dia chi email khong dung dinh dang.")]
    [StringLength(320, ErrorMessage = "Dia chi email khong duoc vuot qua 320 ky tu.")]
    public string DiaChiEmail { get; set; } = string.Empty;
}
