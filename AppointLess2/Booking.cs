//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Booking
    {
        public long Id { get; set; }
        public System.DateTime Time { get; set; }
        public int LengthMinutes { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public byte Status { get; set; }
        public int ScheduleID { get; set; }
        public long TimeSlotID { get; set; }
    
        public virtual Schedule Schedule { get; set; }
        public virtual TimeSlot TimeSlot { get; set; }
    }
}
