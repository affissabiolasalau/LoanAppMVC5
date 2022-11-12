using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    [Keyless]
    public class Login
    {
        [Required(ErrorMessage = "Please Enter Phone Number...")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Phone number is invalid")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter PIN")]
        [RegularExpression("([0-9]*)", ErrorMessage = "PIN must be 4 digits")]
        [StringLength(4, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 4)]
        [Display(Name = "Password")] 
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
