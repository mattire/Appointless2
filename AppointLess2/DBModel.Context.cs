﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppointLess2
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }

        //public System.Data.Entity.DbSet<AppointLess2.Models.ScheduleViewModels.ScheduleVM> ScheduleVMs { get; set; }

        //public System.Data.Entity.DbSet<AppointLess2.Models.ScheduleViewModels.TimeSlotVM> TimeSlotVMs { get; set; }
    }
}
