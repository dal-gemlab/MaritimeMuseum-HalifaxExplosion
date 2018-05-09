using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using FileHelpers;
using Rug.Osc;

namespace StudyControlApp.Model
{
    [DelimitedRecord(",")]
    class ParticipantData
    {
        [FieldConverter(ConverterKind.Date, "hh:mm:ss.fff")]
        public DateTime TimeStamp;
        [FieldArrayLength(3)]
        public float[] Position;
        [FieldArrayLength(4)]
        public float[] Rotation;
        public string GazeTarget;
        public string IsExpanded;
        public int ClickCountSinceLastUpdate;

        public ParticipantData()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public ParticipantData(float[] position, float[] rotation, string gazeTarget, string isExpanded, int clickCountSinceLastUpdate) : this()
        {
            Position = position;
            Rotation = rotation;
            GazeTarget = gazeTarget;
            IsExpanded = isExpanded;
            ClickCountSinceLastUpdate = clickCountSinceLastUpdate;
        }

        public ParticipantData(OscPacket data) : this()
        {
            //TODO: fix this so we don't have to parset o as String anymore...

            var values = ((OscMessage) data).ToArray();
            Position = new [] {(float)values[0], (float)values[1], (float)values[2]};
            Rotation = new [] { (float)values[3], (float)values[4], (float)values[5], (float)values[6] };
            GazeTarget = (string)values[7];
            IsExpanded = (string)values[8];
            ClickCountSinceLastUpdate = (int)values[9];
        }
    }
}
