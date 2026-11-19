using Microsoft.AspNetCore.Mvc;
using Notification.Api.DTOs;
using Notification.Api.Services;

namespace Notification.Api.Controllers;

[ApiController]
[Route("api/campaigns")]
public class CampaignsController(ICampaignService dichVuChienDich) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CampaignDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CampaignDetailResponse>> TaoChienDich(
        [FromBody] CreateCampaignRequest yeuCau,
        CancellationToken tokenHuy)
    {
        var chienDich = await dichVuChienDich.TaoChienDichAsync(yeuCau, tokenHuy);

        return CreatedAtAction(
            nameof(LayChiTietChienDich),
            new { id = chienDich.Ma },
            chienDich);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CampaignResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CampaignResponse>>> LayDanhSachChienDich(
        CancellationToken tokenHuy)
    {
        var chienDichs = await dichVuChienDich.LayDanhSachChienDichAsync(tokenHuy);
        return Ok(chienDichs);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<CampaignDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampaignDetailResponse>> LayChiTietChienDich(
        int id,
        CancellationToken tokenHuy)
    {
        var chienDich = await dichVuChienDich.LayChiTietChienDichAsync(id, tokenHuy);

        if (chienDich is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Khong tim thay chien dich",
                Detail = $"Khong co chien dich voi ma {id}."
            });
        }

        return Ok(chienDich);
    }

    [HttpPost("{id:int}/start")]
    [ProducesResponseType<StartCampaignResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<StartCampaignResponse>(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StartCampaignResponse>> BatDauChienDich(
        int id,
        CancellationToken tokenHuy)
    {
        var ketQua = await dichVuChienDich.BatDauChienDichAsync(id, tokenHuy);

        switch (ketQua.Failure)
        {
            case CampaignStartFailure.None:
                return Accepted(ketQua.Response);
            case CampaignStartFailure.NotFound:
                return NotFound(TaoLoi(404, "Khong tim thay chien dich", ketQua.Response.ThongBao));
            case CampaignStartFailure.NoRecipients:
                return BadRequest(TaoLoi(400, "Chien dich chua co nguoi nhan", ketQua.Response.ThongBao));
            case CampaignStartFailure.AlreadyStarted:
                return Conflict(TaoLoi(409, "Khong the bat dau chien dich", ketQua.Response.ThongBao));
            case CampaignStartFailure.QueueUnavailable:
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ketQua.Response);
            default:
                throw new InvalidOperationException("Trang thai ket qua bat dau chien dich khong hop le.");
        }
    }

    private static ProblemDetails TaoLoi(int maTrangThai, string tieuDe, string chiTiet)
    {
        return new ProblemDetails
        {
            Status = maTrangThai,
            Title = tieuDe,
            Detail = chiTiet
        };
    }
}
