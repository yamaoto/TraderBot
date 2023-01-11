using Prometheus;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnect.Services;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

var binanceConfiguration = builder.Configuration.GetSection("Binance");
builder.Services.Configure<BinanceOptions>(binanceConfiguration);

builder.Services.AddGrpc();
builder.Services.AddSingleton<BinanceMetrics>();
builder.Services.AddTransient<OpenSpotCommand>();
builder.Services.AddTransient<GetFuturesBalanceRequest>();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

var binanceOptions = binanceConfiguration.Get<BinanceOptions>();
builder.Services.AddHttpClient<OpenSpotCommand>()
    .ConfigureHttpClient(client =>
    {
        if (binanceOptions!.UseTestnet) client.BaseAddress = new Uri("https://testnet.binancefuture.com");
    });

builder.Services.AddHealthChecks()
    .AddCheck<RavenDbHealthChecks>(nameof(RavenDbHealthChecks))
    .ForwardToPrometheus();

var app = builder.Build();

app.UseHttpMetrics();
app.UseGrpcMetrics();

app.UseCors("AllowAll");

app.MapGrpcService<SpotService>();

app.MapMetrics();
app.MapHealthChecks("/health");

app.Run();

namespace TraderBot.BinanceConnect
{
    public class Program
    {
    }
}