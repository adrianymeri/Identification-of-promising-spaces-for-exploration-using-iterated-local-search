using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class AlgorithmModel
    {
        public int Iteration { get; set; }
        public double ImprovementPercent { get; set; }
        public int IterationNoResult { get; set; }
        public int HillCliming { get; set; }
        public int Swaps { get; set; }
        public int ChangeTimeSlot { get; set; }
        public int NewHomeBase { get; set; }
        public double Score { get; set; }
    }
}
