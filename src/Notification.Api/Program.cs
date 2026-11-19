using Microsoft.EntityFrameworkCore;
using Notification.Api.Services;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var chuoiKetNoi = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Chua cau hinh ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<ApplicationDbContext>(tuyChon =>
    tuyChon.UseSqlServer(chuoiKetNoi));

builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<INotificationPublisher, RabbitMqPublisher>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
