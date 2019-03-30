using System.Linq;
using DataLayer.EfCode;
using DataLayer.EfClasses;

namespace ServiceLayer.Accounts.Concrete
{
    public class RegistrationService
    {
        private const decimal startBalance = 500;
        private readonly PwContext context;
        public RegistrationService(PwContext context)
        {
            this.context = context;
        }

        public bool AddUser(RegistrationDto dto)
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
                return true;
            }
            return false;
        }

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
