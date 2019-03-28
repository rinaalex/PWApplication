using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Accounts
{
    public class LoginDto
    {
        [Required]
        [Display(Name = "Email:")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
