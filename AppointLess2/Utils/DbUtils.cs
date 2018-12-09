using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AppointLess2.Utils
{
    public class DbUtils
    {
        public static IQueryable<Schedule> GetUserSchedules(Entities db, IPrincipal User) {
            return db.Schedules.Where(s => s.AspNetUser.Email == User.Identity.Name);
        }

    }
}