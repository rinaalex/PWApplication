using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Accounts
{
    /// <summary>
    /// Инкапсулирует информацию о пользователе для авторизации
    /// </summary>
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
