using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.EfClasses
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public decimal Balance { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<Transfer> OutgoingTransactions { get; set; }
        [InverseProperty("Recipient")]
        public virtual ICollection<Transfer> ImcomingTransactions { get; set; }
    }
}
