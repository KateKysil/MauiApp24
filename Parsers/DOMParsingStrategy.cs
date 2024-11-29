using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LogLibrary;

namespace Parsers
{
    public class DOMParsingStrategy : IParsingStrategy
    {
        public List<Subject> Parse(string filePath)
        {
            var subjects = new List<Subject>();
            var document = new XmlDocument();
            document.Load(filePath);

            try
            {
                var days = document.GetElementsByTagName("Day");
                if (days.Count == 0)
                {
                    throw new Exception("The XML file does not contain any <Day> elements.");
                }

                foreach (XmlNode dayNode in days)
                {
                    var dayName = dayNode.Attributes?["Name"]?.Value;
                    if (string.IsNullOrEmpty(dayName))
                    {
                        continue; 
                    }

                    foreach (XmlNode groupNode in dayNode.ChildNodes)
                    {
                        var groupName = groupNode.Attributes?["Name"]?.Value;
                        if (string.IsNullOrEmpty(groupName))
                        {
                            continue; 
                        }

                        foreach (XmlNode subjectNode in groupNode.ChildNodes)
                        {
                            var subjectName = subjectNode.Attributes?["Name"]?.Value;
                            if (string.IsNullOrEmpty(subjectName))
                            {
                                continue; 
                            }

                            var subject = new Subject
                            {
                                Day = dayName,
                                Group = groupName,
                                Name = subjectName,
                                Time = subjectNode["Time"]?.Attributes?["Name"]?.Value
                            };

                            

                            var teachersNode = subjectNode["Teachers"];
                            if (teachersNode != null)
                            
                            {
                                foreach (XmlNode teacherNode in teachersNode.ChildNodes)
                                {
                                    var teacher = new Teacher
                                    {
                                        Name = teacherNode.Attributes?["Name"]?.Value,
                                        Position = teacherNode.Attributes?["Position"]?.Value,
                                        Room = teacherNode.Attributes?["RoomNumber"]?.Value
                                    };

                                    if (string.IsNullOrEmpty(teacher.Name))
                                    {
                                        continue; // Skip invalid teacher
                                    }

                                    subject.Teachers.Add(teacher);
                                }
                            }

                            subjects.Add(subject);
                        }
                    }
                }

                if (subjects.Count == 0)
                {
                    throw new Exception("The XML file does not contain valid subject data.");
                }
            }
            catch (Exception ex)
            {
                // Catch any unexpected structural issues
                throw new Exception($"The XML file has an invalid structure: {ex.Message}");
            }

            return subjects;
        }
    }

}
