using Curriculum_Based_Course_timetabling.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Curriculum_Based_Course_timetabling
{
    class IO
    {
        public static string input = "";

        public static void  Read(string filepath,string fixedFilePath=null)
        {
            input = filepath;
            filepath = "Input/" + filepath+".xml";
            //Create A XML Document 
            XmlDocument xmlDocument = new XmlDocument();
            //Read the XML File  
            xmlDocument.Load(filepath);

            //Create a XML Node List with XPath Expression  
            XmlNode xmlNode = xmlDocument.SelectSingleNode("/instance");

            foreach (XmlNode node in xmlNode)
            {
                //get descriptor values
                if (node.NodeType == XmlNodeType.Element && node.Name == "descriptor")
                {
                    foreach (XmlNode datas in node.ChildNodes)
                    {
                        if(datas.Name=="days")
                        {
                            Instance.days=Int32.Parse(datas.Attributes["value"].InnerText);
                        }
                        if (datas.Name == "periods_per_day")
                        {
                            //Instance.periods_per_day = Int32.Parse(datas.Attributes["value"].InnerText);
                        }
                        if (datas.Name == "daily_lectures")
                        {
                            Instance.min_daily_lectures = Int32.Parse(datas.Attributes["min"].InnerText);
                            Instance.max_daily_lectures = Int32.Parse(datas.Attributes["max"].InnerText);
                        }
                        
                    }
                }
                    //get courses
                    if (node.NodeType == XmlNodeType.Element && node.Name == "courses")
                {
                    foreach (XmlNode course in node.ChildNodes)
                    {
                        //id, teacher, lectures , min_days nuk perdoren, students, double_lectures ka cdohere
                        Course courseObject = new Course(
                            course.Attributes["id"].InnerText,
                             course.Attributes["teacher"].InnerText,
                             Int32.Parse(course.Attributes["lectures"].InnerText),
                             Int32.Parse(course.Attributes["students"].InnerText)
                        );
                        Instance.Courses.Add(courseObject.Id,courseObject);
                    }
                }
                //get rooms
                if (node.NodeType == XmlNodeType.Element && node.Name == "rooms")
                {
                    foreach (XmlNode room in node.ChildNodes)
                    {
                        // id, size, building (nuk perdoret)
                        Room roomObject = new Room(room.Attributes["id"].InnerText)
                        {
                            Size = int.Parse(room.Attributes["size"].InnerText)
                        };
                        Instance.Rooms.Add(roomObject.Id,roomObject);

                    }
                }
                //get curricula
                if (node.NodeType == XmlNodeType.Element && node.Name == "curricula")
                {
                    foreach (XmlNode curriculum in node.ChildNodes)
                    {
                        //id
                        Curriculum curriculumObject = new Curriculum(curriculum.Attributes["id"].InnerText);
                  
                        foreach (XmlNode course in curriculum.ChildNodes)
                        {
                            //ref of course
                            curriculumObject.CoursesId.Add(course.Attributes["ref"].InnerText);

                            Instance.Courses[course.Attributes["ref"].InnerText].curriculumIds.Add(curriculum.Attributes["id"].InnerText);
                        }

                        Instance.Curricula.Add(curriculumObject.Id,curriculumObject);
                    }
                }
                //get constraints
                if (node.NodeType == XmlNodeType.Element && node.Name == "constraints")
                {
                    foreach (XmlNode constraint in node.ChildNodes)
                    {
                        //type,course

                        //if type of constraint is period
                        if(constraint.Attributes["type"].InnerText=="period")
                        {
                           
                                var course = Instance.Courses[constraint.Attributes["course"].InnerText];
                                foreach (XmlNode timeslot in constraint.ChildNodes)
                                {
                                //day,period
                                //New logic: ex. day 0 period 0 then unavailable periods are 0,1,2,3 because the period is one hour equivalent to four 15 minutes periods
                                var period = Int32.Parse(timeslot.Attributes["period"].InnerText);

                                for(int i=0; i<4; i++)
                                {
                                    Timeslot timeslotObject = new Timeslot(Int32.Parse(timeslot.Attributes["day"].InnerText), period*4+i);
                                    course.ConstraintNotAvaliableTimeslots.Add(timeslotObject);
                                }

                             
                                }
                            
                        }
                         //if type of constraint is room 
                        if (constraint.Attributes["type"].InnerText== "room")
                        {
                           
                                var course = Instance.Courses[constraint.Attributes["course"].InnerText];
                                foreach (XmlNode room in constraint.ChildNodes)
                                {
                                    //ref ID of room
                                        Room roomObject = Instance.Rooms[room.Attributes["ref"].InnerText];
                                        course.ConstraintValidRooms.Add(roomObject);

                                }
                            
                        }
                    }
                }
            }

            if(fixedFilePath != null)
            {
                ReadFixedSolution(fixedFilePath);
            }
        }

        public static void Write(List<Assignment> solutions, double score)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            string path = "Output\\"+input+" "+ secondsSinceEpoch.ToString() +" Score "+score+".txt";
            using (StreamWriter writeSolution = new StreamWriter(path))
            {
                foreach (var solution in solutions)
                {
                    writeSolution.WriteLine(solution.course.Id + " " + solution.room.Id.Trim() + " " + solution.timeslot.Day + " " + solution.timeslot.Period);

                }
            }
        }

        public static void ReadFixedSolution(string filepath)
        {
            string filePath = "Input/" + filepath + ".txt";
            string line;
            Assignment assignment = null;
            StreamReader streamReader = new StreamReader(filePath);
            while ((line = streamReader.ReadLine()) != null)
            {
                String[] substr = line.Split(' ');
                if (substr[1] != "-")
                {
                    var course = Instance.Courses[substr[0]];
                    var room = Instance.Rooms[substr[1]];
                    var timeslot = new Timeslot(Int32.Parse(substr[2]), Int32.Parse(substr[3]));
                    assignment = new Assignment(course, room, timeslot);
                    Instance.FixedAssignments.Add(assignment);
                }

            }
        }
       
    }
}
