using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class Loans
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Amount not selected")]
        [Display(Name = "Amount")]
        public int Borrowed { get; set; }
        public int ToPay { get; set; }
        public int Paid { get; set; }
        public int Status { get; set; }

        [Required(ErrorMessage = "Please, Select Duration")]
        [Display(Name = "Days")]
        public string Days { get; set; }

        //Location
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateDue { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
