using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class ConfirmEmail
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Code { get; set; }

        public string Email { get; set; }
        public string Resend { get; set; }
        public string Phone { get; set; }
        public string UrlParameter { get; set; }

        public int IsConfirmed { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
