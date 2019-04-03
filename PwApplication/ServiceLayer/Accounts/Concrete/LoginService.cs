using DataLayer.EfCode;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.Accounts.Concrete
{
    /// <summary>
    /// Предоставляет метод авторизации
    /// </summary>
    public class LoginService
    {
        private readonly PwContext context;

        public LoginService(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Выполняет авторизацию
        /// </summary>
        /// <param name="dto">Учетные данные</param>
        /// <returns>Информация о пользователе</returns>
        public InfoUserDto Login(LoginDto dto)
        {
            var user = context.Users.AsNoTracking().Where(p => p.Email == dto.Email && p.Password == dto.Password).SingleOrDefault();
            if (user != null)
            {
                return new InfoUserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Balance = user.Balance
                };
            }
            else return null;
        }
    }
}
