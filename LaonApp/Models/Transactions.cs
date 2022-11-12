using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class Transactions
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; } = 0;
        public int Status { get; set; } = 0;
        public string Type { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
