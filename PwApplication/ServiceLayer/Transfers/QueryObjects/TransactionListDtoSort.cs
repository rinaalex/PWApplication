using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ServiceLayer.Transfers.QueryObjects
{
    /// <summary>
    /// Определяет направления сортировки списка транзакций
    /// </summary>
    public enum OrderByOptions
    {
        [Display(Name = "By date/time ↓")]
        ByDateTimeAsc = 0,
        [Display(Name = "By date/time ↑")]
        ByDateTimeDesc = 1,
        [Display(Name = "By correspondent ↓")]
        ByCorrespondentAsc = 2,
        [Display(Name = "By correspondent ↑")]
        ByCorrespondentDesc = 3,
        [Display(Name = "By amount ↓")]
        ByAmountAsc = 4,
        [Display(Name = "By amount ↑")]
        ByAmountDesc = 5
    }

    /// <summary>
    /// Обеспечивает сортировку списка транзакций
    /// </summary>
    public static class TransactionListDtoSort
    {
        /// <summary>
        /// Выполняет сортировку списка транзакций
        /// </summary>
        /// <param name="transactions">Список транзакций</param>
        /// <param name="orderByOptions">Параметры сортировки</param>
        /// <returns></returns>
        public static IQueryable<TransactionListDto> OrderTransacionsBy
            (this IQueryable<TransactionListDto> transactions, OrderByOptions orderByOptions)
        {
            switch (orderByOptions)
            {
                case OrderByOptions.ByDateTimeAsc:
                    return transactions.OrderBy(x => x.Timestamp);
                case OrderByOptions.ByDateTimeDesc:
                    return transactions.OrderByDescending(x => x.Timestamp);
                case OrderByOptions.ByCorrespondentAsc:
                    return transactions.OrderBy(x => x.Correspondent);
                case OrderByOptions.ByCorrespondentDesc:
                    return transactions.OrderByDescending(x => x.Correspondent);
                case OrderByOptions.ByAmountAsc:
                    return transactions.OrderBy(x => x.Amount);
                case OrderByOptions.ByAmountDesc:
                    return transactions.OrderByDescending(x => x.Amount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(orderByOptions), orderByOptions, null);
            }
        }
    }
}
