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
                // создаем JWT-токен
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

                // сериализация ответа
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

        // поиск пользователя
        private ClaimsIdentity GetIdentity(LoginDto dto)
        {
            LoginService service = new LoginService(context);
            var user = service.Login(dto);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
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
