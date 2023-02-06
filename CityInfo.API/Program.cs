// We're building a web app which needs to be hosted. To do this, a WebApplicationBuilder can be used
var builder = WebApplication.CreateBuilder(args);

// Add services to the container (dependency injection)
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // send 406 Not Acceptable if client requests unsupported format
}).AddXmlDataContractSerializerFormatters(); // add XML support

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
