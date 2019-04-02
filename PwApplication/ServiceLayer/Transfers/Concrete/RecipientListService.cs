using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;

namespace ServiceLayer.Transfers.Concrete
{
    public class RecipientListService
    {
        private readonly PwContext context;

        public RecipientListService(PwContext context)
        {
            this.context = context;
        }

        public IEnumerable<RecipientListDto>GetRecipientList(int senderId)
        {
            var recipients = context.Users.Where(p => p.UserId != senderId).Select(p => new RecipientListDto
            {
                RecipientId=p.UserId,
                RecipientName = p.UserName
            });
            return recipients;
        }
    }
}
