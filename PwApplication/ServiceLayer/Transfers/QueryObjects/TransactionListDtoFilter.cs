using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Transfers.QueryObjects
{
    /// <summary>
    /// Поля фильтрации списка транзакций
    /// </summary>
    public enum TransactionsFilterBy
    {
        [Display(Name = "All data")]
        NoFilter = 0,
        [Display(Name = "By date/time")]
        ByDateTime = 1,
        [Display(Name = "By correspondent")]
        ByCorrespondent = 2,
        [Display(Name = "By Amount")]
        ByAmount = 3
    }

    /// <summary>
    /// Обеспечивает фильтрацию списка транзакций
    /// </summary>
    public static class TransactionListDtoFilter
    {
        /// <summary>
        /// Выполняет фильтрацию списка транзакций
        /// </summary>
        /// <param name="transactions">Список транзакций</param>
        /// <param name="filterBy">Поле фильтрации</param>
        /// <param name="filterValue">Искомое значение</param>
        /// <returns></returns>
        public static IQueryable<TransactionListDto> FilterTransactionsBy
            (this IQueryable<TransactionListDto> transactions, TransactionsFilterBy filterBy, string filterValue)
        {
            if (string.IsNullOrEmpty(filterValue))
            {
                return transactions;
            }
            switch (filterBy)
            {
                case TransactionsFilterBy.NoFilter:
                    return transactions;
                case TransactionsFilterBy.ByDateTime:
                    var dateTime = DateTime.Parse(filterValue);
                    return transactions.Where(p => p.Timestamp == dateTime.ToString("dd.MM.yyyy MM:HH:ss"));
                case TransactionsFilterBy.ByCorrespondent:
                    return transactions.Where(p => p.Correspondent == filterValue);
                case TransactionsFilterBy.ByAmount:
                    var amount = int.Parse(filterValue);
                    return transactions.Where(p => p.Amount == amount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterValue), filterValue, null);
            }
        }
    }
}
