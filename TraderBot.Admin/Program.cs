using TraderBot.Admin.Services;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpc();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

var app = builder.Build();

app.UseGrpcWeb();

app.UseCors("AllowAll");

app.MapGrpcService<AdminService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");

app.Run();