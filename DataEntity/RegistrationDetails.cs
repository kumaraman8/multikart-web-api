using System.ComponentModel.DataAnnotations;

namespace E_CommerceNet.DataEntity
{
    public class RegistrationDetails
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }       

    }
}
