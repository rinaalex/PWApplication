using System.Linq;
using DataLayer.EfClasses;

namespace ServiceLayer.Transfers.QueryObjects
{
    /// <summary>
    /// Обеспечивает загрузку списка транзакций указанного пользователя
    /// </summary>
    public static class TransactionListDtoSelect
    {
        /// <summary>
        /// Формирует список TransactionListDto из списка Transaction
        /// </summary>
        /// <param name="transactions">Список транзакций</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns></returns>
        public static IQueryable<TransactionListDto> MapTransactionToDto(this IQueryable<Transfer> transactions, int userId)
        {
            return transactions.Where(q => q.SenderId == userId || q.RecipientId == userId).
                Select(p => new TransactionListDto
                {
                    TransferId = p.TransferId,
                    Correspondent = p.SenderId == userId ? p.Recipient.UserName : p.Sender.UserName,
                    Type = p.SenderId == userId ? "Credit" : "Debit",
                    Timestamp = p.Timestamp,
                    Amount = p.Amount,
                    ResultingBalance = p.Operations.Where(r=>r.TransferId==p.TransferId && r.UserId==userId).Select(q=>q.ResultingBalance).FirstOrDefault()
                });
        }
    }
}
