using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace ServiceLayer.Transfers
{
    public class TransactionListDto
    {
        public int TransferId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Timestamp { get; set; }
        public string Correspondent { get; set; }
        public decimal Amount { get; set; } 
        public string Type { get; set; }
        public decimal ResultingBalance { get; set; }
    }
}
