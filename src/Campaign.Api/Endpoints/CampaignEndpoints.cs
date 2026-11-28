using System.ComponentModel.DataAnnotations;
using Campaign.Api.DTOs;
using Campaign.Api.Services;

namespace Campaign.Api.Endpoints;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var campaigns = endpoints.MapGroup("/api/campaigns");

        campaigns.MapPost("", CreateCampaign);
        campaigns.MapGet("", GetCampaigns);
        campaigns.MapGet("/{id:guid}", GetCampaign)
            .WithName("GetCampaignById");
        campaigns.MapGet("/{id:guid}/messages", GetMessages);
        campaigns.MapPost("/{id:guid}/messages", CreateMessage);
        endpoints.MapGet("/api/messages/{id:guid}", GetMessage)
            .WithName("GetMessageById");

        return endpoints;
    }

    private static IResult CreateCampaign(
        CreateCampaignRequest request,
        ICampaignService campaignService)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var campaign = campaignService.Create(request.Name, request.Content);
        var response = CampaignResponse.FromModel(campaign);

        return Results.CreatedAtRoute(
            "GetCampaignById",
            new { id = campaign.Id },
            response);
    }

    private static IResult GetCampaigns(ICampaignService campaignService)
    {
        var campaigns = campaignService.GetAll()
            .Select(CampaignResponse.FromModel)
            .ToArray();

        return Results.Ok(campaigns);
    }

    private static IResult GetCampaign(Guid id, ICampaignService campaignService)
    {
        var campaign = campaignService.GetById(id);
        return campaign is null
            ? Results.NotFound(new { message = $"Campaign '{id}' was not found." })
            : Results.Ok(CampaignResponse.FromModel(campaign));
    }

    private static IResult GetMessages(Guid id, ICampaignService campaignService)
    {
        var messages = campaignService.GetMessages(id);
        return messages is null
            ? Results.NotFound(new { message = $"Campaign '{id}' was not found." })
            : Results.Ok(messages.Select(MessageResponse.FromModel).ToArray());
    }

    private static IResult CreateMessage(
        Guid id,
        CreateMessageRequest request,
        ICampaignService campaignService)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var message = campaignService.CreateMessage(id, request.Content);
        return message is null
            ? Results.NotFound(new { message = $"Campaign '{id}' was not found." })
            : Results.CreatedAtRoute(
                "GetMessageById",
                new { id = message.Id },
                MessageResponse.FromModel(message));
    }

    private static IResult GetMessage(Guid id, ICampaignService campaignService)
    {
        var message = campaignService.GetMessageById(id);
        return message is null
            ? Results.NotFound(new { message = $"Message '{id}' was not found." })
            : Results.Ok(MessageResponse.FromModel(message));
    }

    private static Dictionary<string, string[]> Validate<T>(T request)
    {
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(
            request!,
            new ValidationContext(request!),
            validationResults,
            validateAllProperties: true);

        return validationResults
            .SelectMany(result => result.MemberNames.DefaultIfEmpty(string.Empty)
                .Select(member => new { Member = member, result.ErrorMessage }))
            .GroupBy(error => error.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage ?? "Invalid value.").ToArray());
    }
}
