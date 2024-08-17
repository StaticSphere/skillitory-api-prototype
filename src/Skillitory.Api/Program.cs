using Serilog;
using Skillitory.Api.Bootstrap;

const string corsPolicy = "Skillitory_Cors_Policy";

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration);
});

// Register services
builder.Services.AddApiServices(builder.Configuration, corsPolicy);
var app = builder.Build();

// Register runtime middleware pipeline
app.UseApiRuntime(corsPolicy);

// Run API
app.Run();
