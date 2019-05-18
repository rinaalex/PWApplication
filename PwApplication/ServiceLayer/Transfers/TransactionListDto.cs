using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace ServiceLayer.Transfers
{
    /// <summary>
    /// Инкапсулирует информацию о транзакции для отображения в таблице
    /// </summary>
    public class TransactionListDto
    {
        public int TransferId { get; set; }       
        public string Timestamp { get; set; }
        public string Correspondent { get; set; }
        public decimal Amount { get; set; } 
        public string Type { get; set; }
        public decimal ResultingBalance { get; set; }
    }
}
