using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointLess2.Utils
{
    public class BookingManager
    {
        public static bool CheckIfBookingForSlotIsStillAwailable(Booking book, Entities db) {
            var match = db.Bookings.FirstOrDefault(b => b.TimeSlotID == book.TimeSlotID && b.Time == book.Time);
            if (match == null) { return true; }
            return false;
        }
    }
}