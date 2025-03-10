using System.Security.Claims;

namespace BancoCodesarrolloApp_API.Services.Token
{
    public interface IToken
    {
        public int ObtenerUsuarioId(IEnumerable<Claim> Claims);
        public int? ValidateToken(string token);
    }
}
