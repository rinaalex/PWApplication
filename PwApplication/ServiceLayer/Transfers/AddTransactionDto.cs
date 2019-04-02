using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.Transfers
{
    /// <summary>
    /// Инкапсулирует информацию о транзакции для добавления
    /// </summary>
    public class AddTransactionDto
    {
        public int SenderId { get; set; }
        [Required(ErrorMessage ="Required", AllowEmptyStrings = false)]
        public int RecipientId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }        
        public DateTime Timestamp { get; set; }
    }
}
