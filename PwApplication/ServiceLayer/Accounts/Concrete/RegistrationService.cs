using System.Linq;
using DataLayer.EfCode;
using DataLayer.EfClasses;

namespace ServiceLayer.Accounts.Concrete
{
    /// <summary>
    /// Предоставляет метод регистрации
    /// </summary>
    public class RegistrationService
    {
        private const decimal startBalance = 500;
        private readonly PwContext context;

        public RegistrationService(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Выполняет добавление нового пользователя
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns></returns>
        public User AddUser(RegistrationDto dto)
        {
            if (isUnique(dto.Email))
            {
                User user = new User
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Password = dto.Password,
                    Balance = startBalance
                };
                context.Users.Add(user);
                context.SaveChanges();
                return user;
            }
            return null;
        }

        /// <summary>
        /// Выполянет проверку на уникальность email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool isUnique(string email)
        {
            var user = context.Users.Where(p => p.Email == email).FirstOrDefault();
            if (user!=null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
