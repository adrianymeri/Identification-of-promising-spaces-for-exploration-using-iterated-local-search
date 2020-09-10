using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Course
    {
        public string Id { get; }
        public string Teacher { get; }
        public int Lectures { get; }
        public int Students { get; }
        public List<Timeslot> ConstraintNotAvaliableTimeslots { get; set; }
        public List<Room> ConstraintValidRooms { get; set; }
        public List<string> curriculumIds  { get; set;}

        public Course(string Id,string Ieacher,int Lectures,int Students)
        {
            this.Id = Id;
            this.Teacher = Ieacher;
            this.Lectures = Lectures;
            this.Students = Students;
            ConstraintNotAvaliableTimeslots = new List<Timeslot>();
            ConstraintValidRooms = new List<Room>();
            curriculumIds = new List<string>();
        }

        public List<string> GetCurriculums()
        {
            List<string> curriculumIds = new List<string>();
            foreach (var curriculum in Instance.Curricula)
            {
                if(curriculum.Value.CoursesId.Contains(this.Id))
                {
                    curriculumIds.Add(curriculum.Key);
                }
            }
            return curriculumIds;
        }

        public int GetPeriods()
        {
             int periods = this.Lectures * 3;
            int timeSlots = 0;

            if (this.Lectures%2==0 ) //cift
            {
                timeSlots = (Lectures-2)/2;
         
            }
            else
            {
                timeSlots = (Lectures - 1) / 2;
            }
       
            return timeSlots + periods;

        }

    }
}
