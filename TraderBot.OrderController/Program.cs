using Microsoft.Extensions.Options;
using TraderBot.BinanceConnectProto;
using TraderBot.OrderController.Commands;
using TraderBot.OrderController.Infrastructure;
using TraderBot.OrderController.Services;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.Configure<DependencyOptions>(builder.Configuration.GetSection("Dependency"));
builder.Services.Configure<TradingOptions>(builder.Configuration.GetSection("Trading"));
builder.Services.AddGrpcClient<SpotGrpc.SpotGrpcClient>((services, options) =>
{
    var dependencyOptions = services.GetService<IOptions<DependencyOptions>>();
    options.Address = new Uri(dependencyOptions!.Value.SpotServiceEndpoint);
});
await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);
builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("Telegram"));
builder.Services.AddTransient<ITelegramService, TelegramService>();
builder.Services.AddSingleton<GetExchangeStepSize>();
builder.Services.AddHttpClient<TelegramService>();
builder.Services.AddHttpClient<GetExchangeStepSize>();

builder.Services.AddTransient<CreateOrder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<OrderControllerService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();