using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Solution
    {
        public void GenerateSolution()
        {
            List<Assignment> assignments = new List<Assignment>();
            Dictionary<int, Timeslot> timeslots = new Dictionary<int, Timeslot>();

            checkIfValidCurriculum();
            var random = new Random();


            int days = Schedule.days;
            int periods_per_day = Schedule.periods_per_day;

            int k = 0;
            //filling all timeslots 
            for (int i = 0; i < days; i++)
            {
                for (int j = 0; j < periods_per_day; j++)
                {
                    timeslots.Add(k, new Timeslot() { day = i, period = j });
                    k++; // deri i*j = 30 
                }
            }

            foreach (var course in Schedule.Courses)
            {
                var randRoom = new Room();
                if (course.Value.NotAvaliableTimeslots.Count > 0)
                {
                    random = new Random();
                    var timeslot = new Timeslot();
                    bool validSlot = false;
                    Room room = new Room();
                    while (validSlot == false)
                    {
                        int rnd = random.Next(timeslots.Count - 1);
                        timeslot = timeslots[rnd];
                        validSlot = checkIfTimeslotIsAvailable(timeslot, course.Value);
                        room = Schedule.Rooms.ElementAt(random.Next(Schedule.Rooms.Count - 1)).Value;
                        var rStatus = checkIfRoomIsAvailable(room, course.Value);
                        int[] slots = new int[course.Value.lectures * 3];


                        if (rStatus != true) // checking if room is legal 
                        {

                            validSlot = false; // to continue search
                        }
                        else
                        {
                            Assignment asg = new Assignment()
                            {
                                courseId = course.Value.id,
                                roomID = room.id,
                                day = timeslot.day,
                                period = timeslot.period,
                            };
                            assignments.Add(asg);
                            // Console.WriteLine("Room :" + room.id);
                            validSlot = true;
                        }
                    }
                    Console.WriteLine("Course:" + course.Value.id + " Room:" + room.id + " Day: " + timeslot.day + " Period: " + timeslot.period);

                }
                else
                {
                    random = new Random();
                    var timeslot = new Timeslot();
                    bool validSlot = false;
                    Room room = new Room();
                    while (validSlot == false)
                    {
                        int rnd = random.Next(timeslots.Count - 1);
                        timeslot = timeslots[rnd];
                        //validSlot = checkIfTimeslotIsAvailable(timeslot, course.Value);
                        room = Schedule.Rooms.ElementAt(random.Next(Schedule.Rooms.Count - 1)).Value;

                        var rStatus = checkIfRoomIsAvailable(room, course.Value);
                        int[] slots = new int[course.Value.lectures * 3];


                        if (rStatus != true) // checking if room is legal 
                        {
                            //Assignment asg = new Assignment()
                            //{
                            //    courseId = course.Value.id,
                            //    roomID = room.id,
                            //};
                            //assignments.Add(asg);
                            validSlot = false; // to continue search
                        }
                        else
                        {
                            Assignment asg = new Assignment()
                            {
                                courseId = course.Value.id,
                                roomID = room.id,
                                day = timeslot.day,
                                period = timeslot.period,
                            };
                            assignments.Add(asg);
                            //Console.WriteLine("Room :" + room.id);
                            validSlot = true;
                        }
                    }
                    Console.WriteLine("Course:" + course.Value.id + " Room:" + room.id + " Day: " + timeslot.day + " Period: " + timeslot.period);
                }
                Console.WriteLine("\n-----------------------------------------");
            }
            int a = 0;
            foreach (var asg in assignments)
            {
                a++;
                Console.WriteLine(a+"  Kursi: "+ asg.courseId + " Room: " + asg.roomID + " Day: " + asg.day + " Period: " + asg.period);
            }


        }
       /// <summary>
       /// This function checks period constraint of course
       /// </summary>
       /// <param name="timeslot">Random Timeslot</param>
       /// <param name="course">Course objects to check period constraints</param>
       /// <returns>true or false</returns>
        public bool checkIfTimeslotIsAvailable(Timeslot timeslot,Course course)
        {
            foreach (var item in course.NotAvaliableTimeslots)
            {
                if (timeslot.day != item.day && timeslot.period != item.period)
                {
                    return true;
                }
            }
            return false;
        }

        //TO DO later: 1. Nese kursi ka 3 ligjerata, me se shumti ligj duhet me fillu ne slotin e fundit-2 te dites
        // 2. Kursi ne te njejtin slot kohor mos me kon me ni kurs tjeter ne te njejten dhome
        //3. Currikula e njejt mos me pas 2 kurse ne te njejten slot kohor
        //4.
        public bool checkIfRoomIsAvailable(Room room, Course course)
        {
            foreach (var item in course.NotAvailableRooms)
            {
                if (room.id == item.id)
                {                 
                    return false;
                }              
            }           
            return true;
        }

        //TO DO: Currikula e njejt mos me pas 2 kurse ne te njejten slot kohor
        public bool checkIfValidCurriculum()//Course course1, Course course2
        {
            foreach (var item in Schedule.Curricula)
            {
                foreach (var c in item.Value.CoursesId)
                {
                    Console.WriteLine("C:" + c);
                }
                Console.WriteLine("############");
               
            }
            return false;
        }
    }
}
