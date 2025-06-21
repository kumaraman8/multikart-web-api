using E_CommerceNet.AdventureContext;
using E_CommerceNet.Controllers;
using E_CommerceNet.DataEntity;
using E_CommerceNet.Model.Request;
using E_CommerceNet.Model.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace E_CommerceNet.Services
{
    public interface IRegistrationsServices
    {
        public Task<CommonResponse> AddRegistration(RegistrationRequest request);
    }

    public class RegistrationClass : IRegistrationsServices
    {
        private readonly AdventureDbContext _Dbcontext;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationClass(AdventureDbContext Dbcontext, ILogger<RegistrationController> logger)
        {
            _Dbcontext = Dbcontext;
            _logger = logger;     
        }

        public async Task<CommonResponse> AddRegistration(RegistrationRequest request)
        {
            try
            {
                var Namedata = await _Dbcontext.RegistrationDetails
                               .Where(u => u.Email!.ToLower().Contains(request.Email!.ToLower())).FirstOrDefaultAsync();
                
                if (Namedata != null)
                {
                    return new CommonResponse() { resCode = 403, resData = null, resMessage = "This user already exist" };
                }
                // Hash the password using ASP.NET Core Identity
                var passwordHasher = new PasswordHasher<string>();
                string hashedPassword = passwordHasher.HashPassword(null!, request.Password!);

                var data = new RegistrationDetails
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = hashedPassword,
                    CreatedDate = DateTime.Now,                    
                };
                 _Dbcontext.RegistrationDetails.Add(data);
                _Dbcontext.SaveChanges();
                return new CommonResponse() { resCode = 200, resData = true, resMessage = "Success" };
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return new CommonResponse() { resCode = 501, resData = ex.ToString(), resMessage = "Internal Server error" };
            }
        }
    }
}
