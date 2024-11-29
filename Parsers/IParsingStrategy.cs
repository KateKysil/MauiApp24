using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers
{
    public interface IParsingStrategy
    {
        List<Subject> Parse(string filePath);
    }

}
