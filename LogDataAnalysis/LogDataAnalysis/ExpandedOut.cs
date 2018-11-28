using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace LogDataAnalysis
{
    [DelimitedRecord(",")]
    class ExpandedOut
    {
        public string Condition;
        public double CottonMill;
        public double RichmondSchool;
        public double SugarRefinery;
        public double XYZ;
        public double FrenchWharf;
        public double GravingDock;
        public double BellTower;
        public double PowerPlant;
        public double VeithHouse;
        public double Shipyard;
        public double MulgravePark;
        public double TextBoard;
        public double TotalViewingTime;
        public int TotalBuildingSelected;
        public double TotalTrialTime;

        public ExpandedOut()
        {

        }

        public ExpandedOut(Dictionary<string, TimeSpan> data, TimeSpan textBoardTime, string condition, double totalTrialTime)
        {
            Condition = condition;
            TotalViewingTime = 0;
            foreach (KeyValuePair<string, TimeSpan> keyValuePair in data)
            {
                TotalViewingTime += keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("G_01"))
                    CottonMill = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("G_02"))
                    RichmondSchool = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("G_03"))
                    SugarRefinery = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("G_04"))
                    XYZ = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("G_07"))
                    FrenchWharf = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("09"))
                    GravingDock = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("Group1"))
                    BellTower = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("power"))
                    PowerPlant = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("veith"))
                    VeithHouse = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("06"))
                    Shipyard = keyValuePair.Value.TotalSeconds;
                if (keyValuePair.Key.Contains("mulgrave"))
                    MulgravePark = keyValuePair.Value.TotalSeconds;
            }

            TextBoard = textBoardTime.TotalSeconds;
            TotalBuildingSelected = data.Count;
            TotalTrialTime = totalTrialTime;
        }


    }



}
