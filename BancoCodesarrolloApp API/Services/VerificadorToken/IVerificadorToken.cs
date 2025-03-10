namespace BancoCodesarrolloApp_API.Services.VerificadorToken
{
    public interface IVerificadorToken
    {
        public Task<bool> VerificarTokenAlmacenado();
    }
}
