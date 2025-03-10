using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BancoCodesarrolloApp_API.Services.Token
{
    public class Token : IToken
    {
        private readonly IConfiguration _configuration;

        public Token(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int ObtenerUsuarioId(IEnumerable<Claim> Claims)
        {
            var usuario = Claims.Where(x => x.Type.Equals("UsuarioId")).Select(x => x.Value).FirstOrDefault();
            return Convert.ToInt32(usuario);
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UsuarioId").Value);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
