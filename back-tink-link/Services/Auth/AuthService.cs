using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        try
        {
            var JWT = _configuration.GetSection("JWT")?.Get<ConfiguracoesTokenConfig>();

            if (JWT == null)
                throw new ArgumentNullException("Não foi possível encontrar as configurações do Token JWT.");

            var _tokenHandler = new JwtSecurityTokenHandler();
            var _key = Encoding.UTF8.GetBytes(JWT.Secret);

            var _claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Name),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.NickName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var userRoles = user.Roles.Split("|");

            foreach (var role in userRoles)
                _claims.Add(new Claim("roles", role));

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                expires: DateTime.UtcNow.AddMinutes(JWT.ExpiresIn),
                claims: _claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
            );

            return _tokenHandler.WriteToken(token);
        }
        catch (Exception)
        {
            throw;
        }
    }
}