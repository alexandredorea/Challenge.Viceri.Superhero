using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Viceri.Superhero.Api;

/// <summary>
///
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        var culture = CultureInfo.CreateSpecificCulture("pt-BR");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        builder.Services.AddControllers(option =>
        {
            option.RespectBrowserAcceptHeader = true;
            option.ReturnHttpNotAcceptable = true;
            option.AllowEmptyInputInBodyModelBinding = true;
        });

        builder.Services
            .Configure<RouteOptions>(option => { option.LowercaseUrls = true; })
            .Configure<ApiBehaviorOptions>(option => { option.SuppressModelStateInvalidFilter = true; }); //Suprime a validação automática do ModelState para que o FluentValidation seja o único responsável

        builder.Services.ConfigureHttpJsonOptions(option =>
        {
            option.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            option.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                option.IncludeXmlComments(xmlPath, true);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder.WithOrigins("http://localhost:5173")
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure();

        return builder;
    }
}