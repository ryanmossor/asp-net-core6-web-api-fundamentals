using CityInfo.API;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// We're building a web app which needs to be hosted. To do this, a WebApplicationBuilder can be used
var builder = WebApplication.CreateBuilder(args); // adds config from appsettings.json and appsettings.{env.EnvironmentName}.json

builder.Host.UseSerilog();

// Add services to the container (dependency injection)
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // send 406 Not Acceptable if client requests unsupported format
}).AddNewtonsoftJson() // replace default JSON input/output formatter
  .AddXmlDataContractSerializerFormatters(); // add XML support

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AddTransient: used for lightweight, stateless services
// AddScoped: created once per request
// AddSingleton: created on first request; same instance used for subsequent requests

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

// creates singleton instance of CitiesDataStore, which can be injected into controller instances
// rather than initialized as a singleton within the CitiesDataStore class
builder.Services.AddSingleton<CitiesDataStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
