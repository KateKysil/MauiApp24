using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsers;

namespace Saver
{
    public class XmlSaver : ISaver
    {
        public string GenerateContent(List<Subject> subjects)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<Schedule>");
            foreach (var subject in subjects)
            {
                sb.AppendLine($"  <Day Name=\"{subject.Day}\">");
                sb.AppendLine($"    <Group Name=\"{subject.Group}\">");
                sb.AppendLine($"      <Subject Name=\"{subject.Name}\">");
                sb.AppendLine("        <Teachers>");
                foreach (var teacher in subject.Teachers)
                {
                    sb.AppendLine($"          <Teacher Name=\"{teacher.Name}\" Position=\"{teacher.Position}\" RoomNumber=\"{teacher.Room}\"/>");
                }
                sb.AppendLine("        </Teachers>");
                sb.AppendLine($"        <Time Name=\"{subject.Time}\"/>");
                sb.AppendLine("      </Subject>");
                sb.AppendLine("    </Group>");
                sb.AppendLine("  </Day>");
            }
            sb.AppendLine("</Schedule>");
            return sb.ToString();
        }
    }


}
