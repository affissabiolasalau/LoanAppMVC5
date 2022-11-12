using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class PasswordReset
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Phone number must be numeric")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string Hash { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
