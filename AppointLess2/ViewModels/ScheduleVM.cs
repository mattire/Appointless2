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
        private Schedule sched;

        public ScheduleVM(Schedule sched)
        {
            this.sched      = sched;
            Id              = sched.Id;
            Name            = sched.Name;
            UserID          = sched.UserID;
            DailyStartTime  = sched.StartOfDay;
            DailyEndTime = sched.EndOfDay;
            TimeSlotVMs = sched.TimeSlots.Select(ts=> new TimeSlotVM(ts));
        }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Display(Name = "Nimi")]
        public string Name { get; set; }

        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string UserID { get; set; }

        [Display(Name = "Päivän alku")]
        public TimeSpan? DailyStartTime { get; set; }
        [Display(Name = "Päivän loppu")]
        public TimeSpan? DailyEndTime { get; set; }

        [Display(Name = "Ajat")]
        public IEnumerable<TimeSlotVM> TimeSlotVMs { get; set; }
    }

    public class TimeSlotVM
    {
        private TimeSlot ts;

        public long Id { get; set; }

        [Display(Name = "Viikonpäivät")]
        //public SelectList DaysOfWeek { get; set; }
        public IEnumerable<SelectListItem> DaysOfWeek { get; set; }


        [Required]
        [Display(Name = "Aloitus aika")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
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

        public TimeSlotVM(TimeSlot ts)
        {
            this.ts = ts;
            this.Id             = ts.Id;
            this.TimeOfDay      = ts.TimeOfDay     ;
            this.LengthMinutes  = ts.LengthMinutes ;
            this.ScheduleID     = ts.ScheduleID    ;
            int daysInt = ts.DaysOfWeek;
            var mon = new SelectListItem() { Selected = (daysInt & 1)  != 0, Text = "Ma", Value = "Ma" };
            var tue = new SelectListItem() { Selected = (daysInt & 2)  != 0, Text = "Ti", Value = "Ti" };
            var wed = new SelectListItem() { Selected = (daysInt & 4)  != 0, Text = "Ke", Value = "Ke" };
            var thu = new SelectListItem() { Selected = (daysInt & 8)  != 0, Text = "To", Value = "To" };
            var fri = new SelectListItem() { Selected = (daysInt & 16) != 0, Text = "Pe", Value = "Pe" };
            var sat = new SelectListItem() { Selected = (daysInt & 32) != 0, Text = "La", Value = "La" };
            var sun = new SelectListItem() { Selected = (daysInt & 64) != 0, Text = "Su", Value = "Su" };
            //DaysOfWeek = new SelectList(new[] {mon,tue,wed,thu,fri,sat,sun,});
            DaysOfWeek = new List<SelectListItem>(new[] { mon, tue, wed, thu, fri, sat, sun, });
            
            //var sl = new SelectList();

            //DaysOfWeek = new SelectList(
            //    new[] { "Ma", "Ti", "Ke", "To", "Pe", "La", "Su" });
            
            //this.DaysOfWeek     = ts.DaysOfWeek    ;
        }

        internal TimeSlot ToTS(HttpRequestBase req )
        {
            var ts = new TimeSlot() {
                Id              = Id,
                TimeOfDay       = TimeOfDay,
                LengthMinutes   = LengthMinutes,
                ScheduleID      = ScheduleID,
            };

            //var str = string.Join("", this.DaysOfWeek.Select(d => d.Selected ? "1" : "0"));
            //ts.DaysOfWeek = (byte)Convert.ToInt32(str, 2);

            var binStrLst = new List<string>() { "Su", "La", "Pe", "To", "Ke", "Ti", "Ma" }.Select(s => req.Form.AllKeys.Contains(s) ? "1" : "0");
            var binStr = string.Join("", binStrLst);
            ts.DaysOfWeek = (byte)Convert.ToInt32(binStr, 2);
            return ts;
        }

        internal void SetDaysFromReq(HttpRequestBase req) {
            var mon = new SelectListItem() { Selected = req.Form.AllKeys.Contains("Ma"), Text = "Ma", Value = "Ma" };
            var tue = new SelectListItem() { Selected = req.Form.AllKeys.Contains("Ti"), Text = "Ti", Value = "Ti" };
            var wed = new SelectListItem() { Selected = req.Form.AllKeys.Contains("Ke"), Text = "Ke", Value = "Ke" };
            var thu = new SelectListItem() { Selected = req.Form.AllKeys.Contains("To"), Text = "To", Value = "To" };
            var fri = new SelectListItem() { Selected = req.Form.AllKeys.Contains("Pe"), Text = "Pe", Value = "Pe" };
            var sat = new SelectListItem() { Selected = req.Form.AllKeys.Contains("La"), Text = "La", Value = "La" };
            var sun = new SelectListItem() { Selected = req.Form.AllKeys.Contains("Su"), Text = "Su", Value = "Su" };

            DaysOfWeek = new List<SelectListItem>(new[] { mon, tue, wed, thu, fri, sat, sun, });
        }
    }
}