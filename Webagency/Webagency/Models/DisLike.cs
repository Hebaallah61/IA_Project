//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Webagency.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DisLike
    {
        public int id { get; set; }
        public Nullable<int> articleID { get; set; }
        public Nullable<int> userID { get; set; }
    
        public virtual articlePost articlePost { get; set; }
        public virtual user user { get; set; }
    }
}