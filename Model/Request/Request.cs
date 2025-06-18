using System.ComponentModel.DataAnnotations;

namespace E_CommerceNet.Model.Request
{
    #region RegistrationRequest
    public class RegistrationRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    #endregion

    #region LoginRequest
    public class LoginRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
    #endregion
}
