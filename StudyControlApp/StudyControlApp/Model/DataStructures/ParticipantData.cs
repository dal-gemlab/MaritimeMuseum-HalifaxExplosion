using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace StudyControlApp.Model
{
    [DelimitedRecord(",")]
    class ParticipantData
    {
        [FieldConverter(ConverterKind.Date, "hh:mm:ss.fff")]
        public DateTime TimeStamp;
        public float[] Position;
        public float[] Rotation;
        public string GazeTarget;
        public string IsExpanded;
        public int ClickCountSinceLastUpdate;

        public ParticipantData(float[] position, float[] rotation, string gazeTarget, string isExpanded, int clickCountSinceLastUpdate)
        {
            TimeStamp = DateTime.UtcNow;
            Position = position;
            Rotation = rotation;
            GazeTarget = gazeTarget;
            IsExpanded = isExpanded;
            ClickCountSinceLastUpdate = clickCountSinceLastUpdate;
        }
    }
}
