using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using DataLayer.EfCode;
using ServiceLayer.Accounts.Concrete;
using ServiceLayer.Accounts;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PwWebApp.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly PwContext context;

        public AccountController(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Выполняет создание токена при авторизации пользователя
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns></returns>
        [HttpPost("/token")]
        public async Task Token([FromBody] LoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var identity = GetIdentity(dto);
                if (identity == null)
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Invalid username or password.");
                    return;
                }

                var now = DateTime.UtcNow;
                // Создание JWT-токена
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: identity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = identity.Name
                };

                // Сериализация ответа
                Response.ContentType = "application/json";
                await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            }
            else
            {
                Response.StatusCode = 400;
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                await Response.WriteAsync(message);
                return;
            }
        }
        /// <summary>
        /// Выполняет добавление нового пользователя при решистрации
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns></returns>
        [HttpPost("/registration")]
        public async Task AddUser([FromBody]RegistrationDto dto)
        {
            if (ModelState.IsValid)
            {
                RegistrationService service = new RegistrationService(context);
                if (service.AddUser(dto))
                {
                    await Response.WriteAsync("Successfull registration!");
                }
                else
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("The Email is already taken!");
                }
            }
            else
            {
                Response.StatusCode = 400;
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                await Response.WriteAsync(message);
                return;
            }
        }

        /// <summary>
        /// Выполняет загрузку информации об аторизованном пользователе
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/userInfo")]
        public InfoUserDto GetUserInfo()
        {
            int userId = Convert.ToInt32(User.Identity.Name);
            InfoUserService service = new InfoUserService(context);
            var user = service.GetUserInfo(userId);
            return user;
        }

        /// <summary>
        /// Выполняет поиск пользователя при авторизации
        /// </summary>
        /// <param name="dto">учетные данные пользователя</param>
        /// <returns></returns>
        private ClaimsIdentity GetIdentity(LoginDto dto)
        {
            LoginService service = new LoginService(context);
            var user = service.Login(dto);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserId.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
    }
}
