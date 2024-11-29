using LogLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Parsers
{
    public class SAXParsingStrategy : IParsingStrategy
    {
        public List<Subject> Parse(string filePath)
        {
            var subjects = new List<Subject>();
            Subject currentSubject = null;
            Teacher currentTeacher = null;

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "Day":
                                currentSubject = new Subject
                                {
                                    Day = reader.GetAttribute("Name")
                                };
                                break;
                            case "Group":
                                if (currentSubject != null)
                                    currentSubject.Group = reader.GetAttribute("Name");
                                break;
                            case "Subject":
                                if (currentSubject != null)
                                    currentSubject.Name = reader.GetAttribute("Name");
                                break;
                            case "Teacher":
                                currentTeacher = new Teacher
                                {
                                    Name = reader.GetAttribute("Name"),
                                    Position = reader.GetAttribute("Position"),
                                    Room = reader.GetAttribute("RoomNumber")
                                };
                                break;
                            case "Time":
                                if (currentSubject != null)
                                    currentSubject.Time = reader.GetAttribute("Name");
                                break;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "Teacher" && currentTeacher != null)
                        {
                            currentSubject?.Teachers.Add(currentTeacher);
                            currentTeacher = null;
                        }
                        else if (reader.Name == "Subject" && currentSubject != null)
                        {
                            subjects.Add(currentSubject);
                            currentSubject = null;
                        }
                    }
                }
            }

            return subjects;
        }
    }


}
