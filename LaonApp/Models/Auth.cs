using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace LoanApp.Models
{
    public class Auth
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Email...")]
        [Display(Name = "Email")]
        //        [Remote("IsAccountExists", "Account", ErrorMessage = "The student already exists")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Phone...")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Phone number must be numbers")]
        [StringLength(11, ErrorMessage = "Phone number is not valid.", MinimumLength = 11)]
        [Display(Name = "Phone")]
        //        [Remote("IsAccountExists", "Account", ErrorMessage = "The student already exists")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Enter PIN...")]
        [StringLength(4, ErrorMessage = "The {0} must be {1} characters long.", MinimumLength = 4)]
        [RegularExpression("([0-9]*)", ErrorMessage = "PIN must be numbers")]
        [DataType(DataType.Password)]
        [Display(Name = "PIN")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please Confirm PIN")]
        [RegularExpression("([0-9]*)", ErrorMessage = "PIN must be numbers")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm PIN")]
        [StringLength(4, ErrorMessage = "PIN must be 4 digits.", MinimumLength = 4)]
        [Compare("Password", ErrorMessage = "PIN do not match.")]
        public string RepeatPassword { get; set; }
        public int IsAdmin { get; set; } = 0;
        public int IsModerator { get; set; } = 0;

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}
