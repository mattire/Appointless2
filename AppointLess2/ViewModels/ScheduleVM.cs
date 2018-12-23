using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointLess2.ViewModels.ScheduleViewModels
{
    public class ScheduleVM
    {
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Display(Name = "Nimi")]
        public string Name { get; set; }
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string UserID { get; set; }

        [Display(Name = "Ajat")]
        public List<TimeSlotVM> TimeSlotVMs { get; set; }
    }

    public class TimeSlotVM
    {

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
        public TimeSlotVM()
        {
            DaysOfWeek = new SelectList(
                new[] { "Ma", "Ti", "Ke", "To", "Pe", "La", "Su" });
        }

    }
}