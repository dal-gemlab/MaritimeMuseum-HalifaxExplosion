using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSpanExtractor
{
    class Program
    {
        private static Dictionary<string, TimeSpan> startTimes;
        private static Dictionary<string, TimeSpan> endTimes;

        static void Main(string[] args)
        {
            var startFile = File.ReadAllLines("Start.txt");
            var endFile = File.ReadAllLines("End.txt");

            startTimes = new Dictionary<string, TimeSpan>();
            endTimes = new Dictionary<string, TimeSpan>();

            var currentFile = "";

            foreach (var line in startFile)
            {
                if (line.Contains(@"Files\\GOP"))
                {
                    var fileStartIndex = line.LastIndexOf("\\");
                    var fileEndIndex = line.IndexOf(">");

                    currentFile = 
                        line.Substring(fileStartIndex + 1, fileEndIndex- fileStartIndex-1);
                    
                    startTimes.Add(currentFile, TimeSpan.Zero);
                }
                else if(line.Contains(@"["))
                {
                    var starTimeString = "00:0" + line.Substring(1, 6);
                    var startTime = TimeSpan.Parse(starTimeString);

                    startTimes[currentFile] = startTime;


                }
            }

            foreach (var line in endFile)
            {
                if (line.Contains(@"Files\\GOP"))
                {
                    var fileStartIndex = line.LastIndexOf("\\");
                    var fileEndIndex = line.IndexOf(">");

                    currentFile =
                        line.Substring(fileStartIndex + 1, fileEndIndex - fileStartIndex - 1);

                    endTimes.Add(currentFile, TimeSpan.Zero);
                }
                else if (line.Contains(@"["))
                {
                    var dashPosition = line.IndexOf("-");
                    var endTimeString = "";
                    if(dashPosition > 8 )
                        endTimeString = "00:" + line.Substring(1, 7);
                    else
                        endTimeString = "00:0" + line.Substring(1, 6);

                    var endTime= TimeSpan.Parse(endTimeString);
                    endTimes[currentFile] = endTime;

                }
            }

            List<string> deltas = new List<string>();
            foreach (var endTime in endTimes)
            {
                var fileName = endTime.Key;
                var delta = endTime.Value - startTimes[endTime.Key];

                deltas.Add($"{fileName},{delta.TotalSeconds}");

            }

            File.WriteAllLines("Deltas.csv", deltas);



        }
    }
}
