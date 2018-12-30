using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using AppointLess2.ViewModels;

namespace AppointLess2.Utils
{
    public class DbUtils
    {
        public static IQueryable<Schedule> GetUserSchedules(Entities db, IPrincipal User) {
            return db.Schedules.Where(s => s.AspNetUser.Email == User.Identity.Name);
        }

        internal static Booking CreateBooking(BookingVM model)
        {
            return new Booking()
            {
                //Time          = 
                //LengthMinutes = model.
                Email         = model.Email,
                Name          = model.Name,
                Phone         = model.PhoneNumber,
                Status        = 0,
                ScheduleID    = model.ScheduleId,
                TimeSlotID    = model.TimeSlotId
            };
        }
    }
}