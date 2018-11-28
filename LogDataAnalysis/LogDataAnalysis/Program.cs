using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace LogDataAnalysis
{
    class Program
    {
        private static List<ExpandedOut> expandedOuts;
        private static List<SmallOut> smallOuts;

        static void Main(string[] args)
        {
            var inputEngine = new FileHelperEngine<LogCsv>();
            var outputFolder = "ProcessedLogs";
            Directory.CreateDirectory(outputFolder);
            var files = Directory.GetFiles("Logs");

            expandedOuts = new List<ExpandedOut>();
            smallOuts = new List<SmallOut>();
            foreach (var file in files)
            {
                var records = inputEngine.ReadFile(file).ToList();
                ParseLog(records,Path.GetFileNameWithoutExtension(file));
            }

            var expandedEngine = new FileHelperEngine<ExpandedOut>();
            expandedEngine.HeaderText = expandedEngine.GetFileHeader();
            var smallEngine = new FileHelperEngine<SmallOut>();
            smallEngine.HeaderText = smallEngine.GetFileHeader();

            expandedEngine.WriteFile($"{outputFolder}\\expandedData.csv",expandedOuts);
            smallEngine.WriteFile($"{outputFolder}\\smallData.csv",smallOuts);

        }

        static void ParseLog(List<LogCsv> records, string filename)
        {
            if(records.Count <= 50)
                return;
            records.RemoveAt(0);
            var staringAtExpanded = new Dictionary<string, TimeSpan>();
            var staringAtSmall = new Dictionary<string, TimeSpan>();
            int totalClicks = 0;
            var textBoardTime = new TimeSpan();

            foreach (var record in records)
            {
                totalClicks += record.clicksSinceLast;
                if (record.expanded.Equals("True"))
                {
                    if (staringAtExpanded.ContainsKey(record.target))
                        staringAtExpanded[record.target] += (TimeSpan.FromMilliseconds(250));
                    else
                    {
                        var timespan = TimeSpan.FromMilliseconds(250);
                        staringAtExpanded.Add(record.target, timespan);
                    }
                }
                else if (record.expanded.Equals("False"))
                {
                    if (staringAtSmall.ContainsKey(record.target))
                        staringAtSmall[record.target] += (TimeSpan.FromMilliseconds(250));
                    else
                    {
                        var timespan = TimeSpan.FromMilliseconds(250);
                        staringAtSmall.Add(record.target, timespan);
                    }
                }
                else if (record.expanded.Equals("false") && record.target == "TextBoard")
                {
                    textBoardTime += TimeSpan.FromMilliseconds(250);
                }
            }

            var condition = filename.Split('-')[1];
            var expandedOutEntry = new ExpandedOut(staringAtExpanded,textBoardTime,condition);
            var smallOutEntry = new SmallOut(staringAtSmall,condition);
            expandedOuts.Add(expandedOutEntry);
            smallOuts.Add(smallOutEntry);
            
        }
    }
}
    