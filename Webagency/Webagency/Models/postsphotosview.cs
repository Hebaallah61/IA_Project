using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webagency.Models
{
    public class postsphotosview
    
    {

        public IEnumerable<articlePost> articleposts { get; set; }

        public IEnumerable<photo> photos { get; set; }
    }
}