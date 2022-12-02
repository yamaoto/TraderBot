using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListener.Infrastructure;
using TraderBot.EmailListener.Services;
using TraderBot.OrderControllerProto;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<DependencyOptions>(builder.Configuration.GetSection("Dependency"));
builder.Services.Configure<MailBoxOptions>(builder.Configuration.GetSection("MailBox"));
builder.Services.AddTransient<GetMessagesCommand>();
builder.Services.AddTransient<OpenPositionHandler>();
builder.Services.AddTransient<ClosePositionHandler>();
builder.Services.AddTransient<CollectProcessCommand>();
builder.Services.AddHostedService<SchedulerHostedService>();

builder.Services.AddGrpc();

builder.Services.AddGrpcClient<OrderControllerGrpc.OrderControllerGrpcClient>((services, options) =>
{
    var dependencyOptions = services.GetService<IOptions<DependencyOptions>>();
    options.Address = new Uri(dependencyOptions!.Value.OrderControllerServiceEndpoint);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<EmailListenerService>();

app.Run();