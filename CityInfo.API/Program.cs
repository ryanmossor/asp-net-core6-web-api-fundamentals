using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

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

// Register DB context using dependency injection
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(
        builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"])); // references ConnectionStrings.CityInfoDBConnectionString in appsettings.Development.json

// Inject CityInfoRepository into builder
// AddScoped() best fit for repository -- created once per request
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // scan CityInfo.API assembly for profiles

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.MapControllers();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
