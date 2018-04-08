using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Usermvc.ViewModels
{

    public class vwUSER
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(150)]
        public string Username { get; set; }
        
        public int CountryID { get; set; }
        public int StateID { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email")]        
        [StringLength(150)]
        public string EmailID { get; set; }

        public bool Active { get; set; }

        [DisplayName("Country")]
        public string CountryName { get; set; }

        [DisplayName("State")]
        public string StateName { get; set; }

        public SelectList lstCountry;
        public SelectList lstState;
    }
}