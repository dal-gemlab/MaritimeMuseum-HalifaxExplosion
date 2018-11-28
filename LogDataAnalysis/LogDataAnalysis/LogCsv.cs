using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace LogDataAnalysis
{
    [DelimitedRecord(",")]
    public class LogCsv
    {
        [FieldConverter(ConverterKind.Date, "hh:mm:ss.fff")]
        public DateTime timestamp;

        public float x;
        public float y;
        public float z;

        public float qw;
        public float qx;
        public float qy;
        public float qz;

        public string target;
        public string expanded;
        public int clicksSinceLast;
    }
}
