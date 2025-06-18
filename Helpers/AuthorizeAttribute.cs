using E_CommerceNet.Model.Response;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using E_CommerceNet.DataEntity;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var users = context.HttpContext.Items["Users"] as RegistrationDetails;
        if (users == null)
        {
            /* Not logged in */
            context.Result = new JsonResult(new CommonResponse { resCode = CommonResponseStatus.UNAUTHORIZED, resMessage = CommonResponseMessage.UNAUTHORIZED }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}