using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using csharpapi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ControlConexion>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api Genérica C#",
        Version = "v1",
        Description = "API de prueba con ASP.NET Core y Swagger",
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Email = "soporte@miapi.com",
            Url = new Uri("https://miapi.com/contacto")
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Genérica C#");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseSession();
app.UseAuthorization();
app.MapControllers();

var configuration = builder.Configuration;
string jwtKey = configuration["Jwt:Key"];
Console.WriteLine($"Clave JWT: {jwtKey}");

string connectionString = configuration.GetConnectionString("SqlServer");
Console.WriteLine($"Conexión a BD: {connectionString}");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
