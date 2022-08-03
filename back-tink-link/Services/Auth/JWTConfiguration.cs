using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class JWTConfiguration
{
    public static IServiceCollection AddJWT(this IServiceCollection services, ConfigurationManager configuration)
    {
        try
        {
            var _secret = configuration.GetSection("JWT")
                                                    ?.Get<ConfiguracoesTokenConfig>();

            if (_secret == null)
                throw new ArgumentNullException("Não foi possível encontrar as configurações do Token JWT.");

            var _key = Encoding.ASCII.GetBytes(_secret.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // TODO - desabilitar em produção
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = false, //caso o servidor de autorização seja diferente
                    ValidateAudience = false, //seria o servidor que vai fazer o request da autorização pro Issuer
                    RequireExpirationTime = true
                };
            });

            return services;
        }
        catch (Exception)
        {
            throw;
        }
    }
}