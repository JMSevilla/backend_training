//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace backend_training.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbuser
    {
        public int userID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string isverified { get; set; }
        public string istype { get; set; }
        public string isstatus { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
        public string istoken { get; set; }
    }
}
