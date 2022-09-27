using Microsoft.OpenApi.Models;

public static class SwaggerConfig
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "API tink-links",
                    Version = "v1",
                    Description = "API REST desenvolvida com ASP .NET 6 para o projeto <b>Tinklinks</b>",
                    Contact = new OpenApiContact
                    {
                        Name = "N33 Software",
                    }
                });

            var _security = new OpenApiSecurityRequirement();

            _security.Add(new OpenApiSecurityScheme { BearerFormat = "Bearer {}" }, new string[] { });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Utiliza��o: Escreva 'Bearer {seuToken}'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
                }
            });
        });
    } 
}