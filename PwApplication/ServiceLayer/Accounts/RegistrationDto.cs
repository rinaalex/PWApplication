using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Accounts
{
    /// <summary>
    /// Инкапсулирует информацию о пользователе для авторизации
    /// </summary>
    public class RegistrationDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
