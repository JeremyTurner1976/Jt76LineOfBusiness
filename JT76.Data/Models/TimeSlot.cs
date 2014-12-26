using System;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class TimeSlot : ModelBase
    {
        public TimeSlot()
        {
            IsSessionSlot = true;
        }
        public DateTime Start { get; set; }
        public bool IsSessionSlot { get; set; }

        /// <summary>Duration of session in minutes.</summary>
        public int Duration { get; set; }
    }
}
