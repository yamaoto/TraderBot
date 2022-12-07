using Microsoft.Extensions.Options;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListener.HostedServices;
using TraderBot.EmailListener.Infrastructure;
using TraderBot.OrderControllerProto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DependencyOptions>(builder.Configuration.GetSection("Dependency"));
builder.Services.Configure<MailBoxOptions>(builder.Configuration.GetSection("MailBox"));
builder.Services.AddTransient<OpenPositionHandler>();
builder.Services.AddTransient<ClosePositionHandler>();
builder.Services.AddTransient<CollectProcessCommand>();
builder.Services.AddSingleton<EmailChannel>();
builder.Services.AddSingleton<MailBoxStore>();
builder.Services.AddHostedService<SchedulerHostedService>();
builder.Services.AddHostedService<EmailListenerJob>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<OrderControllerGrpc.OrderControllerGrpcClient>((services, options) =>
{
    var dependencyOptions = services.GetService<IOptions<DependencyOptions>>();
    options.Address = new Uri(dependencyOptions!.Value.OrderControllerServiceEndpoint);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailListener");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();