using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Balance { get; set; }
        public int Bonus { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
