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

        [Display(Name = "Viikonpäivät")]
        public SelectList DaysOfWeek { get; set; }

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