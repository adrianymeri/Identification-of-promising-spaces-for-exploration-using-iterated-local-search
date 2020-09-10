using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccbt.Models
{
    public class Instance
    {
        public static int days, periods_per_day=48, min_daily_lectures, max_daily_lectures;
        public static Dictionary<string,Room> Rooms=new Dictionary<string, Room>();
        public static Dictionary<string,Course> Courses=new Dictionary<string,Course>();
        public static Dictionary<string,Curriculum> Curricula = new Dictionary<string, Curriculum>();
        public static List<Assignment> FixedAssignments = new List<Assignment>();

    }
}
