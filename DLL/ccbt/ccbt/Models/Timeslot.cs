using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccbt.Models
{
    public class Timeslot
    {
        public int Day { get; set; }
        public int Period { get; set; }
       
        public Timeslot(int Day,int Period)
        {
            this.Period = Period;
            this.Day = Day;
        }

        public Timeslot ShallowCopy()
        {
            return (Timeslot)this.MemberwiseClone();
        }
    }
}
