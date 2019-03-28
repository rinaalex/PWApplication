using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class Operation
    {
        public int TransferId { get; set; }
        public int UserId { get; set; }

        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        [Required]
        public decimal ResultingBalance { get; set; }


        public virtual Transfer Transfer { get; set; }
        public virtual User User { get; set; }
    }
}
