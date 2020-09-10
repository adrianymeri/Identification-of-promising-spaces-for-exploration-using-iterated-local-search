using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccbt.Models
{
    public class Curriculum
    {
        public string Id { get; }
        public List<String> CoursesId { get; set; }
        public Curriculum(string Id)
        {
            this.Id = Id;
            CoursesId = new List<string>();
        }
    }
}
