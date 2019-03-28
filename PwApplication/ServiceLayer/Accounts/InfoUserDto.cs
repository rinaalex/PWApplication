using System;

namespace ServiceLayer.Accounts
{
    /// <summary>
    /// Инкапсулирует информацию о пользователе для отображения
    /// </summary>
    public class InfoUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal Balance { get; set; }
    }
}