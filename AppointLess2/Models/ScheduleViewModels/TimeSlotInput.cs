using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.Models.ScheduleViewModels
{
    public class TimeSlotInput
    {
        //AppointLess2.TimeSlot
        public int Id { get; set; }


        public SelectList DaysOfWeek { get; set; }

        //[Display(Name = "Weekdays")]
        //public IEnumerable<string> DaysOfWeek { get; set; } = new List<string> { "Ma", "Ti", "Ke", "To", "Pe", "La", "Su" };
        //[Display(Name = "Ma")]
        //public bool Monday { get; set; }
        //[Display(Name = "Ti")]
        //public bool Tuesday { get; set; }
        //[Display(Name = "Ke")]
        //public bool Wensday { get; set; }
        //[Display(Name = "To")]
        //public bool Thursday { get; set; }
        //[Display(Name = "Pe")]
        //public bool Friday { get; set; }
        //[Display(Name = "La")]
        //public bool Saturday { get; set; }
        //[Display(Name = "Su")]
        //public bool Sunday { get; set; }
        [Required]
        [Display(Name = "Aloitus aika")]
        public System.TimeSpan TimeOfDay { get; set; }
        [Required]
        [Display(Name = "Pituus minuutteina")]
        public int LengthMinutes { get; set; }

        public int ScheduleID { get; set; }
        //public virtual Schedule Schedule { get; set; }
        public TimeSlotInput()
        {
            DaysOfWeek = new SelectList(
                new[] { "Ma", "Ti", "Ke", "To", "Pe", "La", "Su" });

            //SelectedSources = new[] { "Google" };
        }

    }
}