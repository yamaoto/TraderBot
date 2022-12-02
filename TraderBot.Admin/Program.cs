using TraderBot.Admin.Services;
using TraderBot.RavenDb;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

await builder.Services.AddAndConfigureRavenDbAsync(builder.Configuration);

var app = builder.Build();

// app.UseGrpcWeb();

app.MapGrpcService<AdminService>();
    // .EnableGrpcWeb();


app.Run();