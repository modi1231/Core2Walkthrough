
using System;
using System.ComponentModel.DataAnnotations;
 

namespace Core2Walkthrough.Data
{
    public class USERS
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "XP")]
        public int XP { get; set; }

        public DateTime DATE_ENTERED { get; set; }

        public bool IS_ADMIN { get; set; }

        [Display(Name = "Description")]
        public string DESCRIPTION { get; set; }
         
    }
}
