using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BancoCodesarrolloApp_API.Services.VerificadorToken;

namespace BancoCodesarrolloApp_API.Utils.JWT
{
    public class ActionFilterToken : IAsyncActionFilter
    {
        private readonly IVerificadorToken _verificadorToken;

        public ActionFilterToken(IVerificadorToken verificadorToken)
        {
            _verificadorToken = verificadorToken;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var actionDescriptor = context.ActionDescriptor;
            var allowAnonymousAttribute = actionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (allowAnonymousAttribute)
            {
                await next();
            }

            var isValidToken = await _verificadorToken.VerificarTokenAlmacenado();

            if (!isValidToken)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
