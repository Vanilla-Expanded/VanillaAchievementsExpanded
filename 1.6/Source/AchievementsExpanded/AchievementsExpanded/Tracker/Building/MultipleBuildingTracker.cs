using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;


namespace AchievementsExpanded
{
    public class MultipleBuildingTracker:BuildingTracker 
    {
        public MultipleBuildingTracker()
        {
        }

        public MultipleBuildingTracker(MultipleBuildingTracker reference) : base(reference)
        {
            buildingsList = reference.buildingsList;
            builtThings = new List<string>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref buildingsList, "buildingsList", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref builtThings, "builtThings", LookMode.Value);
        }

        public override bool Trigger(Building building)
        {
            if (buildingsList != null)
            {
                if (builtThings.Contains(building.GetUniqueLoadID()))
                    return false;
                else
                    builtThings.Add(building.GetUniqueLoadID());
                bool buildingFound = false;
                foreach (ThingDef buildingEntry in buildingsList)
                {
                    buildingFound = (building.def == buildingEntry && (madeFrom is null || madeFrom == building.Stuff));
                    
                    if (buildingFound) {
                        triggeredCount++;
                        break;
                    }
                }
            }
       
           
            return triggeredCount >= count;
        }

        List<ThingDef> buildingsList = new List<ThingDef>();

        private new int triggeredCount;
        protected List<string> builtThings;
    }
}
