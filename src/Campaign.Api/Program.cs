using Campaign.Api.Endpoints;
using Campaign.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ICampaignService, InMemoryCampaignService>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

var healthResponse = () => Results.Ok(new
{
    service = "Campaign.Api",
    status = "running",
    message = "Campaign API is running"
});

app.MapGet("/", healthResponse);
app.MapGet("/health", healthResponse);
app.MapCampaignEndpoints();

app.Run();
