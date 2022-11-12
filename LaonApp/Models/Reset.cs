using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    [Keyless]
    public class Reset
    {

        [Required(ErrorMessage = "Please Enter New PIN...")]
        [RegularExpression("([0-9]*)", ErrorMessage = "PIN must be numbers")]
        [StringLength(4, ErrorMessage = "The {0} must be 4 characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Confirm PIN...")]
        [RegularExpression("([0-9]*)", ErrorMessage = "PIN must be numbers")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm PIN")]
        [Compare("Password", ErrorMessage = "PIN do not match.")]
        public string RepeatPassword { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
