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

            //return DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)).AddDays(1);
        }

        public static DateTime FromString(String str) {
            return DateTime.ParseExact(str, "yyyy.MM.dd", CultureInfo.InvariantCulture);
        }
    }
}