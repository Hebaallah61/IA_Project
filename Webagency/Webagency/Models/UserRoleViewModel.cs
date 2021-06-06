using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webagency.Models
{
    public class UserRoleViewModel
    {
        //internal string userName;

        public  user user { get; set; }
        public IEnumerable<role> userRoles { get; set; }
    }
}