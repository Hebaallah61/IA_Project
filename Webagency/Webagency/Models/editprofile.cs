using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webagency.Models
{
    public class editprofile

    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Enter UserName")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter first Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Enter Email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Phone Number")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public int PhoneNumber { get; set; }


        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Enter confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name = "confirm Password")]
        public string confirmPass { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Required(ErrorMessage = "User Role")]

        [Display(Name = "User type")]
        public int roleID { get; set; }
    }
}