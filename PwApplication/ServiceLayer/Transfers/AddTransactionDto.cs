using System;

namespace ServiceLayer.Transfers
{
    /// <summary>
    /// Инкапсулирует информацию о транзакции для добавления
    /// </summary>
    public class AddTransactionDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
