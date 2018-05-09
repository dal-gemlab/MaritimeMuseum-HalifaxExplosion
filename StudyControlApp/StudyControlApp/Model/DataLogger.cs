using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using Rug.Osc;

namespace StudyControlApp.Model
{
    class DataLogger
    {
        private FileHelperEngine<ParticipantData> engine;
        private readonly List<ParticipantData> dataEntryList;
        private readonly string filename;

        public DataLogger(string filename)
        {
            this.filename = filename;
            dataEntryList = new List<ParticipantData>();
            engine = new FileHelperEngine<ParticipantData>();
        }

        public void AddData(OscPacket data)
        {
            dataEntryList.Add(new ParticipantData(data));
        }

        public void SaveData()
        {
            engine.WriteFile(filename, dataEntryList);
        }
        }
}
