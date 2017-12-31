using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core2Walkthrough.Data
{
    public class NEWS
    {
        public int ID { get; set; }

        [Display(Name = "Text")]
        public string TEXT { get; set; }

        public DateTime DATE_ENTERED { get; set; }

    }
}
