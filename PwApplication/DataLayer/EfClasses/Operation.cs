using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.EfClasses
{
    public class Operation
    {
        [ForeignKey("Transfer")]
        public int TransferId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        [Required]
        public decimal ResultingBalance { get; set; }

        public virtual Transfer Transfer { get; set; }
        public virtual User User { get; set; }
    }
}
