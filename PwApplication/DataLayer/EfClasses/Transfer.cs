using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.EfClasses
{
    public class Transfer
    {
        public int TransferId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public virtual User Sender { get; set; }

        [ForeignKey("Recipient")]
        public int RecipientId { get; set; }
        public virtual User Recipient { get; set; }

        public ICollection<Operation> Operations { get; set; }
    }
}
