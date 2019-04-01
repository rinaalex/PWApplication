using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Transfers.QueryObjects;
using DataLayer.EfCode;

namespace ServiceLayer.Transfers.Concrete
{
    /// <summary>
    /// Обеспечивает загрузку списка транзакций
    /// </summary>
    public class ListTransactionService
    {
        private readonly PwContext context;

        public ListTransactionService(PwContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Выполняет загрузку списка транзакций
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="options">Параметры фильтрации и сортировки</param>
        /// <returns></returns>
        public IQueryable<TransactionListDto> GetList(int userId, SortFilterOptions options)
        {
            IQueryable<TransactionListDto> transactions =
                context.Transfers.AsNoTracking().MapTransactionToDto(userId).
                FilterTransactionsBy(options.TransactionsFilterByOptions, options.FilterValue).
                OrderTransacionsBy(options.OrderByOptions);
            return transactions;
        }
    }
}
