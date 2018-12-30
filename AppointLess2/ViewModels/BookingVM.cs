using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppointLess2.ViewModels
{
    /// <summary>
    /// Booking User info
    /// </summary>
    public class BookingVM
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Sähköposti osoite", Prompt = "name@gmail.com")]
        [Required(ErrorMessage = "Sähköpostiosoite tarvitaan")]
        public string Email { get; set; }

        [Display(Name = "Nimi")]
        [Required(ErrorMessage = "Nimi tarvitaan")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Puhelin numero")]
        public string PhoneNumber { get; set; }

        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string WeekStartYear { get; set; }
    }

    /// <summary>
    /// Booking Week View data
    /// </summary>
    public class BookingWeekVM
    {
        public DateTime Current               { get; set; } = DateTime.Now;
        public List<Holiday> PersonalHolidays { get; set; }
        public List<DateTime> WeekDays        { get; set; }
        public List<DateTime> Holidays        { get; set; }
        //public IEnumerable<Booking> Bookings  { get; }
        public Schedule Schedule              { get; set; }

        public Dictionary<TimeSlot, Dictionary<DayOfWeek, Booking>> TimeSlotWeekBookingsMap    { get; set; }
        public Dictionary<TimeSlot, Tuple<int?, int?>>              TimeSlotWeekIntBookingsMap { get; set; }

        public BookingVM Booking { get; set; } = new BookingVM();

        private List<string> DayNames { get; set; } = new List<string> { "Ma", "Ti", "Ke", "To", "Pe", "La", "Su " };

        public string GetDayName(DayOfWeek dow) {
            switch (dow)
            {
                case DayOfWeek.Sunday   : return "Su";
                case DayOfWeek.Monday   : return "Ma";
                case DayOfWeek.Tuesday  : return "Ti";
                case DayOfWeek.Wednesday: return "Ke";
                case DayOfWeek.Thursday : return "To";
                case DayOfWeek.Friday   : return "Pe";
                case DayOfWeek.Saturday : return "La";
                default:                          
                    break;
            }
            return "";
        }

        public BookingWeekVM(Schedule schedule, DateTime wkStart, bool intMode = true)
        {
            Schedule = schedule;
            WeekDays = Enumerable.Range(0, 7).Select(i => wkStart.AddDays(i)).ToList();
            Holidays = GetHolidays(WeekDays.First(), WeekDays.Last());
            //Bookings = schedule.Bookings.Where(b => b.Time >= WeekDays.First() && b.Time <= WeekDays.Last());

            if (intMode)
            {
                TimeSlotWeekIntBookingsMap = Schedule.TimeSlots.ToDictionary(ts => ts, ts =>
                {
                    var weekBookings = ts.Bookings.Where(b => b.Time >= WeekDays.First() && b.Time <= WeekDays.Last());
                    IEnumerable<int> daysToInts = weekBookings.Select(b => b.Time.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)b.Time.DayOfWeek)-1);
                    int res = 0;
                    foreach (var di in daysToInts) {
                        res = res | (int)Math.Pow(2, di);
                    }
                    //return (int?)res;

                    return new Tuple<int?, int?>(res, ts.DaysOfWeek);
                    //return new Tuple<int?, int?>(-1, -1);
                });
            }
            else
	        {
                TimeSlotWeekBookingsMap = Schedule.TimeSlots.ToDictionary(ts => ts, ts =>
                {
                    var weekBookings = ts.Bookings.Where(b => b.Time >= WeekDays.First() && b.Time <= WeekDays.Last());
                    return weekBookings.ToDictionary(wb => wb.Time.DayOfWeek, wb => wb);
                    //return new Dictionary<DayOfWeek, Booking>();
                });
            }
        }

        private List<DateTime> GetHolidays(DateTime start, DateTime end)
        {
            IEnumerable<PublicHoliday> publicHolidays = DateSystem.GetPublicHoliday(CountryCode.FI, start, end);
            return publicHolidays.Select(ph => ph.Date).ToList();
        }

    }
}