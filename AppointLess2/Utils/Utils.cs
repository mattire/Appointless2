using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace AppointLess2.Utils
{
    public class Utils
    {
        public static DateTime GetStartOfCurrentWeek() {
            return DateTime.Today.AddDays(-1 * (
                DateTime.Today.DayOfWeek== DayOfWeek.Sunday? 7: (int)DateTime.Today.DayOfWeek)
                ).AddDays(1);
        }

        // Week starting from monday
        public static DateTime WeekStartByDateTime(DateTime dt) {
            var dayNum = WeekDayToIntStartingFromMonday(dt);
            return dt.AddDays(1-dayNum);
        }

        public static int WeekDayToIntStartingFromMonday(DateTime dt) {
            if (dt.DayOfWeek == DayOfWeek.Sunday) { return 7; }
            return (int)dt.DayOfWeek;
        }

        public static string Booking2Str(Booking bkng) {
            string statString;
            switch (bkng.Status)
            {
                case 0:
                    statString = "Vahvistamaton";
                    break;
                case 1:
                    statString = "Vahvistettu";
                    break;
                case 2:
                default:
                    statString = "Määrittämätön";
                    break;
            }
            return string.Join(":", new List<string>() { bkng.Name, bkng.Email, bkng.Phone, statString });
        }

        public static string Booking2StringKey(Booking bkng) {
            var day = WeekDayToIntStartingFromMonday(bkng.Time);
            return day + ":" + bkng.TimeSlot.TimeOfDay.Hours + ":" + bkng.TimeSlot.TimeOfDay.Minutes + ":" + bkng.TimeSlot.LengthMinutes;
        }

        public static DateTime FromString(String str) {
            return DateTime.ParseExact(str, "yyyy.MM.dd", CultureInfo.InvariantCulture);
        }

        public static bool CheckBoxToBool(string cbVal)
        {
            if (string.Compare(cbVal, "false") == 0)
                return false;
            if (string.Compare(cbVal, "true,false") == 0)
                return true;
            else
                throw new ArgumentNullException(cbVal);
        }
    }
}