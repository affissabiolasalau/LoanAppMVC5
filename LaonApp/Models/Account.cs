using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Please Enter First Name...")]
        [Display(Name = "First Name")]
        [DefaultValue(true)]
        public string? FirstName { get; set; }
        

        [Required(ErrorMessage = "Please Enter Last Name...")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Select Gender...")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Name of State Missing...")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Name of City Missing...")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number Missing...")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }


        public string? Photo { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of Birth Missing...")]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "BVN Missing...")]
        [Display(Name = "BVN")]
        public string BVN { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }
    }
}
