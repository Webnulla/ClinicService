using System.Net;
using ClinicService.Data;
using ClinicService.Services;
using ClinicService.Services.Impl;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.RequestHeaders.Add("Authorization");
    logging.RequestHeaders.Add("X-Real-IP");
    logging.RequestHeaders.Add("X-Forwarded-For");
});

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();

}).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });

builder.Services.AddDbContext<ClinicServiceDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
});

builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWhen(
    ctx => ctx.Request.ContentType != "application/grpc",
    builder =>
    {
        builder.UseHttpLogging();
    }
);

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<ClinicService.Services.Impl.ClinicService>();
    endpoints.MapGrpcService<PetService>();
});

app.Run();