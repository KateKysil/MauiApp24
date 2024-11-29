using Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saver
{
    public interface ISaver
    {
        string GenerateContent(List<Subject> subjects);
    }
}
