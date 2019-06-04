using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Authorization;
using ServiceLayer.Accounts.Concrete;
using ServiceLayer.Accounts;

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
        [HttpPost("/api/login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var identity = GetIdentity(dto);
                if (identity == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return NotFound(ModelState);
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
                Microsoft.AspNetCore.Http.CookieOptions options = new Microsoft.AspNetCore.Http.CookieOptions();
                options.Expires = System.DateTime.Now.AddSeconds(30);
                Response.Cookies.Append("token", response.access_token, options);
                Response.Cookies.Append("userName", response.username, options);
                return Ok(response);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        /// <summary>
        /// Выполняет добавление нового пользователя при решистрации
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns></returns>
        [HttpPost("/api/registration")]
        public IActionResult Registration([FromBody]RegistrationDto dto)
        {
            if (ModelState.IsValid)
            {
                RegistrationService service = new RegistrationService(context);
                var user = service.AddUser(dto);
                if (user!=null)
                {
                    return Ok(user);
                }
                else
                {
                    ModelState.AddModelError("", "The Email is already taken!");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Выполняет загрузку информации об аторизованном пользователе
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/api/account")]
        public IActionResult Account()
        {
            int userId = Convert.ToInt32(User.Identity.Name);
            InfoUserService service = new InfoUserService(context);
            var user = service.GetUserInfo(userId);
            return Ok(user);
        }

        /// <summary>
        /// Выполняет загрузку списка получателей для указанного пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("/api/account/recipients")]
        public IActionResult GetRecipientList()
        {
            RecipientListService service = new RecipientListService(context);
            var senderId = Convert.ToInt32(User.Identity.Name);
            var recipients = service.GetRecipientList(senderId);
            return Ok(recipients);
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
