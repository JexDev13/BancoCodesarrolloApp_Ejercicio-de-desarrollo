using BancoCodesarrolloApp_API.DTO.Usuario;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BancoCodesarrolloApp_API.Utils.JWT
{
    public class JwtToken
    {
        public static UserToken BuildToken(UserCredentials user, string jwtKey, int userId)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, user.Email),
                new("UsuarioId", $"{userId}"),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(8);

            JwtSecurityToken token = new(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
