using BancoCodesarrolloApp_API.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BancoCodesarrolloApp_API.Services.VerificadorToken
{
    public class VerificadorToken : IVerificadorToken
    {
        private readonly BankContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public VerificadorToken(BankContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<bool> VerificarTokenAlmacenado()
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["JWT:key"])),
                ClockSkew = TimeSpan.Zero
            };

            var usuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext!.User.Claims.Where(x => x.Type.Equals("UsuarioId")).Select(x => x.Value).FirstOrDefault());

            var jti = _httpContextAccessor.HttpContext!.User.Claims.Where(x => x.Type.Equals("jti")).Select(x => x.Value).FirstOrDefault();

            var sesion = await _context.Sessions.FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            if (sesion == null) return false;

            if (sesion != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    var principal = tokenHandler.ValidateToken(sesion.Token, validationParameters, out var validatedToken);
                    var jwtTokenInfo = (JwtSecurityToken)validatedToken;
                    var jtiAlmacenado = jwtTokenInfo.Claims.First(claim => claim.Type == "jti").Value;

                    if (!jtiAlmacenado.Equals(jti))
                    {
                        return false;
                    }
                }
                catch (SecurityTokenException ex)
                {
                    Console.WriteLine($"Error al validar el token: {ex.Message}");

                }
            }
            return true;
        }
    }
}
