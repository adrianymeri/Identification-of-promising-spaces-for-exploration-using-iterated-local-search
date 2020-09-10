using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Curriculum_Based_Course_timetabling.Models
{
    class Solution
    {

        public List<Assignment> assignments = new List<Assignment>();
        public static int days = Instance.days;
        readonly int periods_per_day = Instance.periods_per_day;
        public List<AlgorithmModel> AlgorithmModels { get; set; }

        public List<Assignment> GenerateSolution()
        {

            List<Course> anotherList = new List<Course>();
            //order by curricule
            foreach (var cur in Instance.Curricula)
            {
                foreach (var j in cur.Value.CoursesId)
                {
                    if (anotherList.Where(x => x.Id == j).Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        anotherList.Add(Instance.Courses[j]);
                    }
                }
            }
            foreach (var c in Instance.Courses.Where(x => x.Value.GetCurriculums().Count < 1))
            {
                anotherList.Add(c.Value);
            }

            Assignment assignment = null;
            Restart:
            foreach (var ass in Instance.FixedAssignments)
            {
                assignments.Add(ass);
            }
            foreach (var course in anotherList.Where(p => !Instance.FixedAssignments.Any(p2 => p2.course.Id == p.Id)))
            {
                bool hardConstraints = false;
                bool status = true;
                List<int> daysList = new List<int> { 0, 1, 2, 3, 4 };
                while (hardConstraints == false)
                {

                    int randDay = 0;
                    Timeslot timeslot = null;
                    Random random = new Random();

                    //find valid room
                    var r = new List<Room>();
                    Room room = null;
                    foreach (var i in Instance.Rooms.Values)
                    {
                        if (course.ConstraintValidRooms.Where(x => x.Id == i.Id).Count() < 1)
                        {
                            r.Add(i);
                        }
                    }
                    if (r.Where(x => x.Size >= course.Students).Count() > 0)
                    {
                        int size = r.Where(x => x.Size >= course.Students).Count();
                        room = r.Where(x => x.Size >= course.Students).ElementAt(random.Next(0, size));
                    }
                    else
                    {
                        room = r.ElementAt(random.Next(0, r.Count()));
                    }

                    if (daysList.Count < 1)
                    {
                        assignments.Clear();
                        goto Restart;
                    }
                    var g = random.Next(0, daysList.Count());
                    randDay = daysList.ElementAt(g);
                    daysList.RemoveAt(g);

                    if (assignments.FirstOrDefault(x => x.timeslot.Day == randDay) != null)
                    {
                        for (int i = 0; i < periods_per_day - course.GetPeriods(); i++)
                        {
                            timeslot = new Timeslot(randDay, i);
                            if (CheckIfTimeslotIsAvailable(timeslot, course) == false)
                            {
                                continue;
                            }
                            assignment = new Assignment(course, room, timeslot);
                            if (SlotRoomConstraint(assignment) == false || SlotCurriculaConstraint(assignment) == false || SlotTeacherConstraint(assignment) == false)
                            {
                                if (i == periods_per_day - course.GetPeriods() - 1)
                                {
                                    hardConstraints = false;
                                    break;
                                }
                                continue;
                            }
                            else
                            {
                                hardConstraints = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < periods_per_day - course.GetPeriods(); i++)
                        {
                            timeslot = new Timeslot(randDay, i);
                            if (CheckIfTimeslotIsAvailable(timeslot, course) == true)
                            {
                                assignment = new Assignment(course, room, timeslot);
                                hardConstraints = true;
                                break;
                            }
                        }
                    }
                }
                if (status == true)
                {
                    assignments.Add(assignment);
                }

            }
            return assignments;
        }

        public bool CheckIfRoomIsAvailable(Room room, Course course)
        {
            var notallowed = Instance.Courses[course.Id].ConstraintValidRooms.FirstOrDefault(x => x.Id == room.Id);
            if (notallowed != null)
            {
                return false;

            }
            return true;
        }

        public bool CheckIfTimeslotIsAvailable(Timeslot timeslot, Course course)
        {
            var notallowed = Instance.Courses[course.Id].ConstraintNotAvaliableTimeslots.FirstOrDefault(x => x.Day == timeslot.Day && x.Period >= timeslot.Period && x.Period <= (timeslot.Period + course.GetPeriods()));
            if (notallowed != null)
            {
                return false;

            }
            return true;
        }

        public bool SlotRoomConstraint(Assignment assignment)
        {
            var x = assignments.FirstOrDefault(
              previousAssignments =>
              ((assignment.timeslot.Period <= previousAssignments.timeslot.Period && assignment.timeslot.Period + assignment.course.GetPeriods() - 1 >= previousAssignments.timeslot.Period) ||
                    (assignment.timeslot.Period >= previousAssignments.timeslot.Period && assignment.timeslot.Period <= previousAssignments.timeslot.Period + previousAssignments.course.GetPeriods() - 1)
                    ) &&
                    assignment.room.Id == previousAssignments.room.Id &&
                    assignment.timeslot.Day == previousAssignments.timeslot.Day
              );
            if (x != null)
            {
                return false;
            }
            return true;
        }

        public bool SlotCurriculaConstraint(Assignment assignment)
        {
            var x = assignments.FirstOrDefault(
           previousAssignments =>
           ((assignment.timeslot.Period <= previousAssignments.timeslot.Period && assignment.timeslot.Period + assignment.course.GetPeriods() - 1 >= previousAssignments.timeslot.Period) ||
                 (assignment.timeslot.Period >= previousAssignments.timeslot.Period && assignment.timeslot.Period <= previousAssignments.timeslot.Period + previousAssignments.course.GetPeriods() - 1)
                 ) && Instance.Courses[previousAssignments.course.Id].curriculumIds.Intersect(Instance.Courses[assignment.course.Id].curriculumIds).Count() > 0
                &&
                 assignment.timeslot.Day == previousAssignments.timeslot.Day
           );
            if (x != null)
            {
                return false;
            }
            return true;
        }

        public bool SlotTeacherConstraint(Assignment assignment)
        {
            var x = assignments.FirstOrDefault(
          previousAssignments =>
          ((assignment.timeslot.Period <= previousAssignments.timeslot.Period && assignment.timeslot.Period + assignment.course.GetPeriods() - 1 >= previousAssignments.timeslot.Period) ||
                (assignment.timeslot.Period >= previousAssignments.timeslot.Period && assignment.timeslot.Period <= previousAssignments.timeslot.Period + previousAssignments.course.GetPeriods() - 1)
                ) && assignment.course.Teacher == previousAssignments.course.Teacher
               &&
                assignment.timeslot.Day == previousAssignments.timeslot.Day
          );
            if (x != null)
            {
                return false;
            }
            return true;
        }

        public double GetScore()
        {
            double score = 0;
            score = Cost.GetWindowViolation(assignments) + Cost.GetRoomCapacityPenalty(assignments);
            return score;
        }

        public void SwapRoomMutation()
        {
            var r = new Random();
            var status = true;
            var randAssignment1s = assignments.Where(x => x.course.Students > x.room.Size);
            if (randAssignment1s.Count() < 1)
            {
                return;
            }
            int rnd1 = r.Next(randAssignment1s.Count());
            var randAssignment1 = randAssignment1s.ElementAt(rnd1);

            var differentSlotAssignments = assignments.Where(x => x.timeslot.Day == randAssignment1.timeslot.Day && x.timeslot.Period == randAssignment1.timeslot.Period && x.course.Id != randAssignment1.course.Id && x.room.Size > randAssignment1.course.Students && randAssignment1.room.Size > x.course.Students && randAssignment1.course.GetPeriods() == x.course.GetPeriods());
            // var differentSlotAssignments = assignments.Where(x => x.timeslot.Day == randAssignment1.timeslot.Day && x.timeslot.Period == randAssignment1.timeslot.Period && randAssignment1.course.GetPeriods() == x.course.GetPeriods() && x.room.Size>=randAssignment1.course.Students);
            if (differentSlotAssignments.Count() > 0)
            {
                int rnd2 = r.Next(differentSlotAssignments.Count());
                var randAssignment2 = differentSlotAssignments.ElementAt(rnd2);

                if (CheckIfRoomIsAvailable(randAssignment2.room, randAssignment1.course) == false || CheckIfRoomIsAvailable(randAssignment1.room, randAssignment2.course) == false)
                {
                    status = false;
                }

                var room1 = randAssignment1.room;
                var room2 = randAssignment2.room;
                if (status == true)
                {
                    assignments.First(x => x.course.Id == randAssignment1.course.Id).room = room2;
                    assignments.First(x => x.course.Id == randAssignment2.course.Id).room = room1;
                }
            }
        }

        public void ChangeTimeslotMutation()
        {
            Random random = new Random();

            Solution newSolution = new Solution();
            newSolution = newSolution.Copy(assignments);
            Assignment candidateAssginment = null;

            int day = random.Next(days);
            var assignmentss = newSolution.assignments.Where(p => !Instance.FixedAssignments.Any(p2 => p2.course.Id == p.course.Id));
            int rnd = random.Next(assignmentss.Count());
            var assignment = assignmentss.ElementAt(rnd);

            int timeslot = random.Next(periods_per_day - assignment.course.GetPeriods() + 1);

            if (day != assignment.timeslot.Day)
            {
                candidateAssginment = new Assignment(assignment.course, assignment.room, assignment.timeslot);

                candidateAssginment.timeslot.Day = day;
                candidateAssginment.timeslot.Period = timeslot;
                newSolution.assignments.Remove(assignment);

                if (newSolution.CheckIfTimeslotIsAvailable(candidateAssginment.timeslot, candidateAssginment.course) == false || newSolution.SlotRoomConstraint(candidateAssginment) == false || newSolution.SlotCurriculaConstraint(candidateAssginment) == false || newSolution.SlotTeacherConstraint(candidateAssginment) == false)
                {
                    newSolution = newSolution.Copy(assignments);
                }
                else
                {
                    assignments.First(x => x.course.Id == assignment.course.Id).timeslot.Day = day;
                    assignments.First(x => x.course.Id == assignment.course.Id).timeslot.Period = timeslot;
                }
            }

        }

        public void Tweak()
        {
            ChangeTimeslotMutation();
        }

        public void Perturb(out int swaps, out int changeTimeSlots, int? oldSwaps, int? oldChangeTimeSlot)
        {
            swaps = changeTimeSlots = 0;

            var r = new Random();

            for (int i = 0; i < 10; i++)
            {
                if (oldChangeTimeSlot.HasValue && oldSwaps.HasValue)
                {
                    if (changeTimeSlots < oldChangeTimeSlot)
                    {
                        changeTimeSlots++;
                        ChangeTimeslotMutation();
                    }
                    else if (swaps < oldSwaps)
                    {
                        swaps++;
                        SwapCourses();
                    }
                }
                else
                {
                    int a = r.Next();
                    if (a % 2 == 0)
                    {
                        changeTimeSlots++;
                        ChangeTimeslotMutation();
                    }
                    else
                    {
                        swaps++;
                        SwapCourses();
                    }
                }
              

            }


        }

        public void ChangeRoomMutation()
        {
            Random random = new Random();

            Solution newSolution = new Solution();
            newSolution = newSolution.Copy(assignments);
            Assignment candidateAssginment = null;


            var weak = newSolution.assignments.Where(x => x.course.Students > x.room.Size);

            int rnd = random.Next(weak.Count());
            candidateAssginment = new Assignment(newSolution.assignments.ElementAt(rnd).course, newSolution.assignments.ElementAt(rnd).room, newSolution.assignments.ElementAt(rnd).timeslot);
            var goodRooms = Instance.Rooms.Where(x => x.Value.Size >= candidateAssginment.course.Students);

            rnd = random.Next(goodRooms.Count());

            candidateAssginment.room = goodRooms.ElementAt(rnd).Value;

            newSolution.assignments.RemoveAll(x => x.course.Id == candidateAssginment.course.Id);

            if (newSolution.CheckIfRoomIsAvailable(candidateAssginment.room, candidateAssginment.course) == false || newSolution.SlotRoomConstraint(candidateAssginment) == false)
            {
                newSolution = newSolution.Copy(assignments);
            }
            else
            {
                assignments.First(x => x.course.Id == candidateAssginment.course.Id).room = goodRooms.ElementAt(rnd).Value;

            }
        }

        /// <summary>
        /// List as parameter is copied in actual Solution
        /// </summary>
        /// <returns>Copied List</returns>
        public Solution Copy(List<Assignment> assignments)
        {
            List<Assignment> newList = new List<Assignment>();
            foreach (var item in assignments)
            {
                Course c = new Course(
                    item.course.Id, item.course.Teacher, item.course.Lectures, item.course.Students);
                Room r = new Room(item.room.Id);
                r.Size = Instance.Rooms[item.room.Id].Size;
                Timeslot t = new Timeslot(item.timeslot.Day, item.timeslot.Period);

                Assignment a = new Assignment(
                    c, r, t
                    );

                newList.Add(a);
            }

            Solution solution = new Solution
            {
                assignments = newList
            };
            return solution;
        }


        public void SwapCourses()
        {
            Solution newSolution = new Solution();
            newSolution = newSolution.Copy(assignments);

            var rnd = new Random();
            var dayIndex = rnd.Next(days);
            var daySolutions = assignments.Where(x => x.timeslot.Day == dayIndex).ToList();
            var sameCurricula = Cost.GetWindows(daySolutions, dayIndex);

            if (sameCurricula == null)
            {
                // check for all days
                for (int i = 0; i < days; i++)
                {
                    dayIndex = i;
                    daySolutions = assignments.Where(x => x.timeslot.Day == dayIndex).ToList();
                    sameCurricula = Cost.GetWindows(daySolutions, dayIndex);

                    if (sameCurricula != null)
                    {
                        break;
                    }
                }

                if (sameCurricula == null)
                {
                    this.Copy(newSolution.assignments);
                }
            }

            var hasDifference = false;
            if (sameCurricula != null)
            {
                for (int i = 1; i < sameCurricula.Count; i++)
                {
                    var difference = sameCurricula[i].timeslot.Period - (sameCurricula[i - 1].timeslot.Period + sameCurricula[i - 1].course.GetPeriods());
                    if (difference > 1 || hasDifference)
                    {
                        hasDifference = true;
                        var sol = daySolutions.First(x => x.course.Id == sameCurricula[i].course.Id);
                        FindSlot(dayIndex, sol);

                    }
                }
            }
        }

        public bool FindSlot(int dayIndex, Assignment sol)
        {

            Timeslot timeslot = null;
            Assignment assignment = null;
            bool status = false;
            Solution s = new Solution();
            s = s.Copy(assignments);

            s.assignments.RemoveAll(x => x.course.Id == sol.course.Id);

            Room room = null;

            bool hardConstraints = false;
            for (int j = 0; j < days; j++)
            {
                if (j == dayIndex)
                    continue;
                for (int i = 0; i < periods_per_day - sol.course.GetPeriods(); i++)
                {
                    timeslot = new Timeslot(j, i);
                    if (CheckIfTimeslotIsAvailable(timeslot, sol.course) == false)
                    {
                        continue;
                    }
                    room = FindRoom(timeslot, sol.course, s);
                    if (room == null)
                    {
                        continue;
                    }
                    assignment = new Assignment(sol.course, room, timeslot);

                    if (s.SlotCurriculaConstraint(assignment) == false || s.SlotTeacherConstraint(assignment) == false)
                    {

                        if (i == periods_per_day - sol.course.GetPeriods() - 1)
                        {
                            status = false;
                            hardConstraints = false;
                            break;
                        }

                        status = false;
                        hardConstraints = false;
                        continue;
                    }
                    else
                    {
                        status = true;
                        hardConstraints = true;
                        break;
                    }
                }

                if (status == true)
                {
                    hardConstraints = true;
                    break;
                }
            }

            if (hardConstraints == true)
            {
                s.assignments.Add(assignment);
                assignments.First(x => x.course.Id == assignment.course.Id).timeslot = assignment.timeslot.ShallowCopy();
                assignments.First(x => x.course.Id == assignment.course.Id).room = assignment.room.ShallowCopy();

                return true;
            }
            else
            {
                return false;
            }
        }

        public Room FindRoom(Timeslot t, Course c, Solution s)
        {
            var roomList = new List<Room>();
            Room room = null;
            Assignment assignment = null;
            c = Instance.Courses.Values.First(x => x.Id == c.Id);

            foreach (var i in Instance.Rooms.Values)
            {
                if (c.ConstraintValidRooms.Where(x => x.Id == i.Id).Count() < 1)
                {
                    roomList.Add(i);
                }
            }
            if (roomList.Where(x => x.Size >= c.Students).Count() > 0)
            {
                roomList = roomList.Where(x => x.Size >= c.Students).ToList();
            }
            foreach (var r in roomList)
            {
                assignment = new Assignment(c, r, t);
                if (s.SlotRoomConstraint(assignment))
                {
                    room = r;
                    break;
                }
            }
            return room;
        }
    }
}