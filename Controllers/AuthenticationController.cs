using E_CommerceNet.AdventureContext;
using E_CommerceNet.DataEntity;
using E_CommerceNet.Helpers;
using E_CommerceNet.Model.Request;
using E_CommerceNet.Model.Response;
using E_CommerceNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private AdventureDbContext _dbcontext;
        private readonly ILogger<AuthenticationController> _logger;
        private IAuthenticationServices _Login;
        private Appsettings _appsettings;
        public AuthenticationController(AdventureDbContext dbcontext, ILogger<AuthenticationController> logger, IAuthenticationServices Login, IOptions<Appsettings> appsettings)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _Login = Login;
            _appsettings = appsettings.Value;
        }

        #region LoginAuth
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAuth(LoginRequest request)
        {
            var currentRequest = Request;
            int errorCode = 0;

            #region IP Address

            var ip = currentRequest.Headers["X-Forwarded-For"].FirstOrDefault();
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            ipAddress = ipAddress + " / " + ip;
            _logger.LogInformation($"[{currentRequest.Path}] => API Access From IP: [{ipAddress}]");

            #endregion

            try
            {
                var response = await _Login.LoginAuth(request);
                if (response == null)
                {
                    return BadRequest(new CommonResponse { resCode = 400, resMessage = "Bad Request!!" });
                }
                else
                {
                    return StatusCode(response.resCode, response);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"ERROR CODE [{errorCode}] | [{currentRequest.Path}] | IP ADDRESS [{ipAddress}] : Internel server error !";
                _logger.LogError(errorMsg + " | " + ex.Message + " | " + ex.InnerException);
                _logger.Log(LogLevel.Information, "ok ji");
                return StatusCode(500, new CommonResponse { resCode = 501, resMessage = errorMsg });
            }
        }
        #endregion

        #region ExtendToken
        [Authorize] // ensures token is valid
        [HttpPost("extendToken")]
        public IActionResult ExtendToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return Unauthorized(new CommonResponse { resCode = 401, resMessage = "Unauthorized" });

            // Extract user claims
            var userId = identity.FindFirst("id")?.Value;
            var userName = identity.FindFirst("Name")?.Value;
            var email = identity.FindFirst("Email")?.Value;

            if (userId == null || userName == null || email == null)
                return BadRequest(new CommonResponse { resCode = 400, resMessage = "Invalid Token Data" });

            // Create new token
            var newToken = GenerateJwtToken(new RegistrationDetails
            {
                Id = Convert.ToInt32(userId),
                Name = userName,
                Email = email
            });

            return Ok(new CommonResponse
            {
                resCode = 200,
                resMessage = "Token Extended",
                resData = new { Token = newToken }
            });
        }

        private string GenerateJwtToken(RegistrationDetails users, int hours = 1)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.scret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", users.Id.ToString()),
                    new Claim("Name", users.Name!),
                    new Claim("Email", users.Email!)
                }),
                Expires = DateTime.UtcNow.AddHours(hours), // default 1 hour, can be customized
                Issuer = _appsettings.Issuer,
                Audience = _appsettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
    #endregion
}