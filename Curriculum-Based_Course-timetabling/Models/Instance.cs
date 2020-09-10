using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Instance
    {
        public static int days, periods_per_day=48, min_daily_lectures, max_daily_lectures;
        public static Dictionary<string,Room> Rooms=new Dictionary<string, Room>();
        public static Dictionary<string,Course> Courses=new Dictionary<string,Course>();
        public static Dictionary<string,Curriculum> Curricula = new Dictionary<string, Curriculum>();
        public static List<Assignment> FixedAssignments = new List<Assignment>();

        public static void Reset()
        {
            Instance.days = Instance.min_daily_lectures = Instance.max_daily_lectures = 0;
            Instance.periods_per_day = 48;

            Instance.Rooms = new Dictionary<string, Room>();
            Instance.Courses = new Dictionary<string, Course>();
            Instance.Curricula = new Dictionary<string, Curriculum>();
            Instance.FixedAssignments = new List<Assignment>();
        }
    }
}
