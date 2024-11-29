using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers
{
    public class ScheduleService
    {
        private readonly IParsingStrategy _parsingStrategy;

        public ScheduleService(IParsingStrategy parsingStrategy)
        {
            _parsingStrategy = parsingStrategy;
        }

        public List<Subject> LoadSchedule(string filePath)
        {
            return _parsingStrategy.Parse(filePath);
        }
    }

}
