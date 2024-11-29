
using System.Xml.Linq;
namespace Parsers
{
    public class LINQParsingStrategy : IParsingStrategy
    {
        public List<Subject> Parse(string filePath)
        {
            var subjects = new List<Subject>();
            var document = XDocument.Load(filePath);
            try
            {
                var days = document.Descendants("Day");

                foreach (var day in days)
                {
                    foreach (var subjectElement in day.Descendants("Subject"))
                    {
                        var subject = new Subject
                        {
                            Day = day.Attribute("Name")?.Value,
                            Group = subjectElement.Parent?.Attribute("Name")?.Value,
                            Name = subjectElement.Attribute("Name")?.Value,
                            Time = subjectElement.Element("Time")?.Attribute("Name")?.Value,
                            Teachers = subjectElement.Element("Teachers")?.Descendants("Teacher").Select(t => new Teacher
                            {
                                Name = t.Attribute("Name")?.Value,
                                Position = t.Attribute("Position")?.Value,
                                Room = t.Attribute("RoomNumber")?.Value
                            }).ToList() ?? new List<Teacher>()
                        };

                        subjects.Add(subject);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return subjects;
        }
    }

}
