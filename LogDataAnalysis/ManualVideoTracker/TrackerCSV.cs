using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace ManualVideoTracker
{
    [DelimitedRecord(",")]
    class TrackerCSV
    {
        public double HL_x;
        public double HL_y;
               
        public double nHL_x;
        public double nHL_y;
    }
}
