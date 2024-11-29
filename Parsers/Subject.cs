using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers
{
    public class Subject
    {
        public string Group { get; set; }
        public string Day { get; set; }
        public string Name { get; set; }
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
        public string Time { get; set; }
    }

    public class Teacher
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Room { get; set; }
    }
}
