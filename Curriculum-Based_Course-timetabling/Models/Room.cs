using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Room
    {
        public string Id { get; set; }
        public int Size { get; set; }

        public Room(string Id)
        {
            this.Id = Id;
        }

        public Room ShallowCopy()
        {
            return (Room)this.MemberwiseClone();
        }

    }
}
