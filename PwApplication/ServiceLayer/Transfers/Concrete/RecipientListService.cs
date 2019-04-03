using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.Transfers.Concrete
{
    /// <summary>
    /// Предоставляет метод загрузки списка получателей
    /// </summary>
    public class RecipientListService
    {
        private readonly PwContext context;

        public RecipientListService(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Загружает список получателей для авторизованного пользователя
        /// </summary>
        /// <param name="senderId">Отправитель</param>
        /// <returns></returns>
        public IEnumerable<RecipientListDto>GetRecipientList(int senderId)
        {
            var recipients = context.Users.AsNoTracking().Where(p => p.UserId != senderId).Select(p => new RecipientListDto
            {
                RecipientId=p.UserId,
                RecipientName = p.UserName
            });
            return recipients;
        }
    }
}
