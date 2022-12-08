using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListener.HostedServices;
using TraderBot.EmailListener.Infrastructure;
using TraderBot.OrderControllerProto;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DependencyOptions>(builder.Configuration.GetSection("Dependency"));
builder.Services.Configure<MailBoxOptions>(builder.Configuration.GetSection("MailBox"));
builder.Services.AddTransient<OpenPositionHandler>();
builder.Services.AddTransient<ClosePositionHandler>();
builder.Services.AddTransient<CollectProcessCommand>();
builder.Services.AddSingleton<EmailChannel>();
builder.Services.AddHostedService<SchedulerHostedService>();
builder.Services.AddHostedService<EmailListenerJob>();

await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

builder.Services.AddGrpcClient<OrderControllerGrpc.OrderControllerGrpcClient>((services, options) =>
{
    var dependencyOptions = services.GetService<IOptions<DependencyOptions>>();
    options.Address = new Uri(dependencyOptions!.Value.OrderControllerServiceEndpoint);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();