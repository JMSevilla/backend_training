using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_training.Models
{
    public class Data
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string address { get; set; }
        public char status { get; set; }
        public DateTime createdat { get; set; }
    }
}