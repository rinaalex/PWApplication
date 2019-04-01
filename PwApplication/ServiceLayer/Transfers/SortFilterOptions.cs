using ServiceLayer.Transfers.QueryObjects;

namespace ServiceLayer.Transfers
{
    /// <summary>
    /// Инкапсулирует параметры для списка транзакций
    /// </summary>
    public class SortFilterOptions
    {
        public OrderByOptions OrderByOptions { get; set; }
        public TransactionsFilterBy TransactionsFilterByOptions { get; set; }
        public string FilterValue { get; set; }
    }
}
