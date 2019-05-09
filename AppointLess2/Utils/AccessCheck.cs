using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace AppointLess2.Utils
{
    public class AccessCheck
    {
        public static bool IsEntityCreator(Schedule schedule, IPrincipal principal)
        {
            var Id = principal.Identity.GetUserId();

            System.Diagnostics.Debug.WriteLine(Id);
            System.Diagnostics.Debug.WriteLine(schedule.UserID);
            return principal.Identity.Name == schedule.AspNetUser.Email;
            //return schedule.UserID == principal.Identity.GetUserId();
        }

        public static bool IsEntityCreator(TimeSlot timeSlot, IPrincipal principal)
        {
            return IsEntityCreator(timeSlot.Schedule, principal);
        }

        public static HttpStatusCodeResult CheckEntity(Schedule schedule, IPrincipal principal)
        {
            if (schedule == null)                           { return new HttpNotFoundResult(); }
            else if (!IsEntityCreator(schedule, principal)) { return new HttpUnauthorizedResult(); }
            return null;
        }

        public static HttpStatusCodeResult CheckEntity(TimeSlot timeSlot, IPrincipal principal)
        {
            if (timeSlot == null)                           { return new HttpNotFoundResult(); }
            else if (!IsEntityCreator(timeSlot, principal)) { return new HttpUnauthorizedResult(); }
            return null;
        }


    }
}