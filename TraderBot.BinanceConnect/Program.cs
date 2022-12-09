using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnect.Services;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

var binanceConfiguration = builder.Configuration.GetSection("Binance");
builder.Services.Configure<BinanceOptions>(binanceConfiguration);

builder.Services.AddGrpc();
builder.Services.AddTransient<OpenSpotCommand>();
builder.Services.AddTransient<GetFuturesBalanceRequest>();
await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

var binanceOptions = binanceConfiguration.Get<BinanceOptions>();
builder.Services.AddHttpClient<OpenSpotCommand>()
    .ConfigureHttpClient(client =>
    {
        if (binanceOptions!.UseTestnet) client.BaseAddress = new Uri("https://testnet.binancefuture.com");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SpotService>();

app.Run();

namespace TraderBot.BinanceConnect
{
    public class Program
    {
    }
}