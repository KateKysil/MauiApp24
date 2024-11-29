using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsers;

namespace Saver
{
    public class HtmlSaver : ISaver
    {
        public string GenerateContent(List<Subject> subjects)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title>Schedule</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Schedule</h1>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Day</th><th>Group</th><th>Subject</th><th>Teachers</th><th>Cabinets</th><th>Time</th></tr>");
            foreach (var subject in subjects)
            {
                var teachers = string.Join("; ", subject.Teachers.Select(t => $"{t.Name} ({t.Position})"));
                var cabinets = string.Join("; ", subject.Teachers.Select(t => t.Room));
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{subject.Day}</td>");
                sb.AppendLine($"<td>{subject.Group}</td>");
                sb.AppendLine($"<td>{subject.Name}</td>");
                sb.AppendLine($"<td>{teachers}</td>");
                sb.AppendLine($"<td>{cabinets}</td>");
                sb.AppendLine($"<td>{subject.Time}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
    }

}
