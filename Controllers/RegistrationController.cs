using E_CommerceNet.AdventureContext;
using E_CommerceNet.Model.Request;
using E_CommerceNet.Model.Response;
using E_CommerceNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;


namespace E_CommerceNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly AdventureDbContext _Dbcontext;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IRegistrationsServices _registration;
        public RegistrationController(AdventureDbContext DbContext, ILogger<RegistrationController> logger, IRegistrationsServices registration)
        {
            _Dbcontext = DbContext;
            _logger = logger;
            _registration = registration;           
        }

        [HttpPost("AddRegistration")]
        public async Task<IActionResult> AddRegistrationAsync(RegistrationRequest request) 
        {
            var currentRequest = HttpContext.Request;

            int errorCode = 0;

            #region IP Address

            var ip = currentRequest.Headers["X-Forwarded-For"].FirstOrDefault();
            string remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            string ipAddress = !string.IsNullOrEmpty(ip) ? $"{remoteIpAddress} / {ip}" : remoteIpAddress;
            _logger.LogInformation($"[{currentRequest.Path}] => API Access From IP: [{ipAddress}]");

            #endregion

            //var token = Request.Headers["Authorization"].FirstOrDefault();
            //var tokenArray = token?.Split(" ");
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var jwtToken = tokenHandler.ReadJwtToken(tokenArray?[1]);
            //var LoginId = (jwtToken.Claims.First(x => x.Type == "LoginId").Value);

            try
            {
                var response = await _registration.AddRegistration(request);
                if (response == null)
                {
                    return BadRequest(new CommonResponse { resCode = 400, resMessage = "Bad Request!!" });
                }
                else
                {
                    return Ok(new CommonResponse { resCode = 200, resData = response, resMessage = "Success!!" });
                }
            }

            catch (Exception ex)
            {
                string errorMsg = $"ERROR CODE [{errorCode}] | [{currentRequest.Path}] | IP ADDRESS [{ipAddress}] : Internel server error !";
                _logger.LogError(errorMsg + " | " + ex.Message + " | " + ex.InnerException);
                return StatusCode(500, new CommonResponse
                {
                    resCode = 501,
                    resMessage = errorMsg
                });
            }
        }
    }
}
