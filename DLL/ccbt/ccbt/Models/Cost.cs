using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccbt.Models
{
    public class Cost
    {
            public static int GetWindowViolation(List<Assignment> assignments)
            {
                //Each time window in a curriculum counts as many violation as its length (in periods).
                int window = 0;
                for (int day1 = 0; day1 < Instance.days; day1++)
                {
                    foreach (var curriculum in Instance.Curricula)
                    {
                        List<Assignment> sameCurriculumAssignment = new List<Assignment>();
                        foreach (var assignment in assignments.Where(x => x.timeslot.Day == day1))
                        {
                            if (curriculum.Value.CoursesId.Contains(assignment.course.Id))
                            {
                                sameCurriculumAssignment.Add(assignment);
                            }
                        }

                        var y = sameCurriculumAssignment.OrderBy(x => x.timeslot.Period).ToList();
                        for (int j = 1; j < y.Count; j++)
                        {
                            var difference = y[j].timeslot.Period - (y[j - 1].timeslot.Period + y[j - 1].course.GetPeriods());
                            if (difference > 0)
                            {
                                window += difference;
                            }
                        }
                    }
                }
                return window;
            }

            public static int GetRoomCapacityPenalty(List<Assignment> assignments)
            {
                //Room-Capacity which is soft, and each student above the capacity counts as 1 point of penalty
                int penalty = 0;
                foreach (var assignment in assignments)
                {
                    int difference = assignment.course.Students - assignment.room.Size;
                    if (difference > 0)
                    {
                        penalty += difference;
                    }
                }
                return penalty;
            }

            public static List<Assignment> GetWindows(List<Assignment> solutions, int day)
            {
                foreach (var curricula in Instance.Curricula)
                {
                    var sameCurricula = new List<Assignment>();

                    foreach (var s in solutions)
                    {

                        if (curricula.Value.CoursesId.Contains(s.course.Id))
                        {
                            sameCurricula.Add(s);
                        }
                    }

                    sameCurricula = sameCurricula.OrderBy(x => x.timeslot.Period).ToList();

                    for (int i = 1; i < sameCurricula.Count; i++)
                    {
                        var difference = sameCurricula[i].timeslot.Period - (sameCurricula[i - 1].timeslot.Period + sameCurricula[i - 1].course.GetPeriods());
                        if (difference > 1)
                        {
                            return sameCurricula;
                        }
                    }
                }

                return null;
            }
    }
}
