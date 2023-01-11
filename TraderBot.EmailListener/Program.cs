using Microsoft.Extensions.Options;
using Prometheus;
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

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

builder.Services.AddGrpcClient<OrderControllerGrpc.OrderControllerGrpcClient>((services, options) =>
{
    var dependencyOptions = services.GetService<IOptions<DependencyOptions>>();
    options.Address = new Uri(dependencyOptions!.Value.OrderControllerServiceEndpoint);
});

builder.Services.AddHealthChecks()
    .AddCheck<RavenDbHealthChecks>(nameof(RavenDbHealthChecks))
    .ForwardToPrometheus();

var app = builder.Build();

app.UseHttpMetrics();
app.UseGrpcMetrics();

app.UseCors("AllowAll");

app.MapMetrics();
app.MapHealthChecks("/health");

app.Run();