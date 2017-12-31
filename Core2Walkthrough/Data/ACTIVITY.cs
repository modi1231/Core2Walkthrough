using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core2Walkthrough.Data
{
    public class ACTIVITY
    {
        public int ID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "XP")]
        public int XP { get; set; }

        [Display(Name = "Cool Down Time")]
        public int? COOL_OFF_MINUTES { get; set; } //in case anyone puts nothing in there.

        public bool IS_ACTIVE { get; set; }

        [Display(Name = "Created Date")]
        public DateTime DATE_ENTERED { get; set; }

        [NotMapped]
        public int? TIME_LEFT { get; set; }

        public ACTIVITY()
        {
            COOL_OFF_MINUTES = 0;
        }
    }
}
