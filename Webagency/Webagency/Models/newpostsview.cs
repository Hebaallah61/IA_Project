using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Webagency.Models
{
    public class newpostsview
    {
        
            public int Id { get; set; }

            [Required(ErrorMessage = "Enter Title")]
            [Display(Name = "Title")]
            public string articleTitle { get; set; }

            [Required(ErrorMessage = "Enter Body")]
            [Display(Name = "Article")]
            public string articleBody { get; set; }


            [Required(ErrorMessage = "Enter type")]
            [Display(Name = "Type")]
            public string articleType { get; set; }

            public string photo { get; set; }
        }
    }
