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
        //[Key]
        //public long Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Sähköposti osoite", Prompt = "name@gmail.com")]
        [Required(ErrorMessage = "Sähköpostiosoite tarvitaan")]
        public string Email { get; set; }

        [Display(Name = "Nimi")]
        [Required(ErrorMessage = "Nimi tarvitaan")]
        public string BookerName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Puhelin numero")]
        //[Required(ErrorMessage = "Puhelinnumero tarvitaan")]
        public string PhoneNumber { get; set; }

        public string EventDate     { get; set; }
        //public string EventTime     { get; set; }
        //public string WeekStartYear { get; set; }

        //public int    ScheduleId { get; set; }
        public long   TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }
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
        public List<int>      HolidaysInt     { get; set; }
        public List<int>      PersonalInt     { get; set; }
        //public IEnumerable<Booking> Bookings  { get; }
        public Schedule Schedule              { get; set; }

        public Dictionary<TimeSlot, Dictionary<DayOfWeek, Booking>> TimeSlotWeekBookingsMap    { get; set; }
        public Dictionary<TimeSlot, Tuple<Tuple<int?, int?>, int?>>              TimeSlotWeekIntBookingsMap { get; set; }
        //public Dictionary<TimeSlot, Tuple<int?, int?>> TimeSlotWeekIntBookingsMap { get; set; }

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

        private void GetPersonalHolidayWeekDays() {
            List<int> wDays = new List<int>();
            foreach (var ph in PersonalHolidays)
            {
                var phWeekDays = WeekDays.Where(d => d >= ph.startDate && d <= ph.endDate);
                var wdInts = phWeekDays.Select(p => p.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)p.DayOfWeek);
                foreach (var wd in wdInts)
                {
                    if (!wDays.Contains(wd)) { wDays.Add(wd); }
                }
            }
            PersonalInt = wDays;
        }

        public BookingWeekVM(Schedule schedule, DateTime wkStart, bool intMode = true)
        {
            Schedule = schedule;
            WeekDays = Enumerable.Range(0, 7).Select(i => wkStart.AddDays(i)).ToList();
            Holidays = GetHolidays(WeekDays.First(), WeekDays.Last());

            // Check overlapping with users holidasys
            PersonalHolidays = schedule.AspNetUser
                .Holidays.Where(h=>h.startDate < WeekDays.Last() &&
                                   WeekDays.First() < h.endDate).ToList();
             GetPersonalHolidayWeekDays();


            // National holidays
            HolidaysInt = Holidays.Select(
                h => ((int)h.DayOfWeek) == 0 ? 7 : (int)h.DayOfWeek).ToList();
            //Bookings = schedule.Bookings.Where(b => b.Time >= WeekDays.First() && b.Time <= WeekDays.Last());

            if (intMode)
            {
                TimeSlotWeekIntBookingsMap = Schedule.TimeSlots.ToDictionary(ts => ts, ts =>
                {
                    var weekBookings 
                        = ts.Bookings.Where(
                                b => b.Time >= WeekDays.First() && 
                                b.Time <= WeekDays.Last());

                    var confirmedWeekBookings 
                            = weekBookings.Where(b => b.Status >= 1);

                    IEnumerable<int> bkngsDaysToInts = weekBookings
                                            .Select(b => b.Time.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)b.Time.DayOfWeek)-1);

                    IEnumerable<int> confBkngsDaysToInts = confirmedWeekBookings
                                            .Select(b => b.Time.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)b.Time.DayOfWeek) - 1);

                    int res = 0;
                    foreach (var di in bkngsDaysToInts) {
                        res = res | (int)Math.Pow(2, di);
                    }
                    int res2 = 0;
                    foreach (var di in confBkngsDaysToInts)
                    {
                        res2 = res2 | (int)Math.Pow(2, di);
                    }

                    return new Tuple<Tuple<int?,int?>, int?>(
                        new Tuple<int?, int?>(res,res2), ts.DaysOfWeek);
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