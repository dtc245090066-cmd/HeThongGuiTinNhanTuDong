using System.ComponentModel.DataAnnotations;
using Campaign.Api.DTOs;
using Campaign.Api.Services;

namespace Campaign.Api.Endpoints;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var campaigns = endpoints.MapGroup("/api/campaigns");

        campaigns.MapPost("/", CreateCampaign);
        campaigns.MapGet("/", GetCampaigns);

        return endpoints;
    }

    private static IResult CreateCampaign(
        CreateCampaignRequest request,
        ICampaignService campaignService)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        if (!isValid)
        {
            var errors = validationResults
                .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
                    .Select(member => new { Member = member, result.ErrorMessage }))
                .GroupBy(error => error.Member)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage ?? "Invalid value.").ToArray());

            return Results.ValidationProblem(errors);
        }

        var campaign = campaignService.Create(request.Name, request.Content);
        var response = CampaignResponse.FromModel(campaign);

        return Results.Created("/api/campaigns", response);
    }

    private static IResult GetCampaigns(ICampaignService campaignService)
    {
        var campaigns = campaignService.GetAll()
            .Select(CampaignResponse.FromModel)
            .ToArray();

        return Results.Ok(campaigns);
    }
}
