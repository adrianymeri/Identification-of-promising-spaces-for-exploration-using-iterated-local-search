using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Assignment
    {
        public Course course;
        public Room room;
        public Timeslot timeslot;

        public Assignment(Course course,Room room,Timeslot timeslot)
        {
            this.course = course;
            this.room = room;
            this.timeslot=timeslot;
       
        }
    }
}
