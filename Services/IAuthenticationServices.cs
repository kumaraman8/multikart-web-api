using E_CommerceNet.AdventureContext;
using E_CommerceNet.Controllers;
using E_CommerceNet.DataEntity;
using E_CommerceNet.Helpers;
using E_CommerceNet.Model.Request;
using E_CommerceNet.Model.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceNet.Services
{
    public interface IAuthenticationServices
    {
        public RegistrationDetails getProfile(int LoginId);
        public Task<CommonResponse> LoginAuth(LoginRequest request);
    }

    public class Login : IAuthenticationServices
    {
        private AdventureDbContext _dbContext;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly Appsettings _appsettings;
        public Login(AdventureDbContext dbContext, ILogger<AuthenticationController> logger, IOptions<Appsettings> appsettings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appsettings = appsettings.Value;
        }

        #region getProfile
        public RegistrationDetails getProfile(int LoginId)
        {
            return _dbContext.RegistrationDetails.Where(u => u.Id == LoginId).FirstOrDefault();
        }
        #endregion

        #region LoginAuth
        public async Task<CommonResponse> LoginAuth(LoginRequest request)
        {
            var userData = await _dbContext.RegistrationDetails.Where(u=>u.Email == request.Email).FirstOrDefaultAsync();

            // Verify the hashed password
            var passwordHasher = new PasswordHasher<string>();
            var result = passwordHasher.VerifyHashedPassword(null!, userData.Password!, request.Password!);

            if (result != PasswordVerificationResult.Success)
            {
                return new CommonResponse()
                {
                    resCode = 403,
                    resData = null,
                    resMessage = "Invalid email or password"
                };
            }

            if (userData == null)
            {
                return new CommonResponse() { resCode = 403, resData = null, resMessage = "Data Not Found" };
            }
            else
            {
                var TokenData = GenerateJwtToken(userData);
                var res = new LoginAuthResponseModel()
                {
                    Id = userData.Id,
                    Name = userData.Name,
                    Token = TokenData,
                    Email = userData.Email
                };
                return new CommonResponse() { resCode = 200, resData = res, resMessage = "LoginSuccessfull" };
            }

        }

        private string GenerateJwtToken(RegistrationDetails users)
        {
            /* Generate token that is valid for 1 month */
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.scret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {     new Claim("id", users.Id.ToString()),
                      new Claim("Name", users.Name!),
                      new Claim("Email", users.Email.ToString()!),
                      //new Claim("VerticalId", users.VerticalId.ToString()!)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _appsettings.Issuer,
                Audience = _appsettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
