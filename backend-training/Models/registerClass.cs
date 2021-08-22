using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_training.Models
{
    public class registerClass
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public char isverified { get; set; }
        public char istype { get; set; }
        public char isstatus { get; set; }
        public DateTime createdat { get; set; }

    }
}