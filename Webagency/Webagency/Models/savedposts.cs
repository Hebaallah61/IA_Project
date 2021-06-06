using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webagency.Models
{
    public class savedposts
    {
        public IEnumerable<savePost> savePosts { get; set; }
        public IEnumerable<articlePost> articlePosts { get; set; }
    }
}