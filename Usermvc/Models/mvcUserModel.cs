using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Usermvc.Models
{
    //public class COUNTRY
    //{
    //    public int id { get; set; }
    //    public int Country { get; set; }
    //}

    //public class STATE
    //{
    //    public int id { get; set; }
    //    public int StateName { get; set; }
    //}

    public class USER
    {
        public int id { get; set; }
        public string Username { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string EmailID { get; set; }
        public bool Active { get; set; }        
    }
}