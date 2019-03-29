using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PwWebApp
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:55659/"; // потребитель токена
        const string KEY = "secretkey12345678"; // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена в минутах
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
