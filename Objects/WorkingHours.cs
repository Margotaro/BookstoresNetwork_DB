using System;
using System.Collections.Generic;

namespace BookStore
{
    public sealed class WorkingHours
    {
        public WorkingHours(string shiftname, TimeSpan start_of_the_shift, TimeSpan end_of_the_shift)
        {
            this.shiftname = shiftname;
            this.start_of_the_shift = start_of_the_shift;
            this.end_of_the_shift = end_of_the_shift;
        }
        public string shiftname { get; }
        public TimeSpan start_of_the_shift { get; }
        public TimeSpan end_of_the_shift { get; }
        public static List<WorkingHours> Shifts = new List<WorkingHours>(2) { new WorkingHours("first", new TimeSpan(8, 0, 0), new TimeSpan(14, 0, 0)), new WorkingHours("second", new TimeSpan(14, 0, 0), new TimeSpan(20, 0, 0)) }; 
    }
}
