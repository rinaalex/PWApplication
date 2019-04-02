using System.Linq;
using Microsoft.EntityFrameworkCore;
using DataLayer.EfCode;

namespace ServiceLayer.Accounts.Concrete
{
    /// <summary>
    /// Обеспечивает загрузку информации о пользователе
    /// </summary>
    public class InfoUserService
    {
        private readonly PwContext context;

        public InfoUserService(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Выполняет загрузку данных о пользователе из азы данных
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        public InfoUserDto GetUserInfo(int userId)
        {
            var user = context.Users.AsNoTracking().Where(p => p.UserId == userId).SingleOrDefault();
            if (user != null)
            {
                return new InfoUserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Balance = user.Balance
                };
            }
            else
                return null; 
        }
    }
}
