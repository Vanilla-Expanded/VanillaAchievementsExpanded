using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;


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
            if (Scribe.mode == LoadSaveMode.PostLoadInit && builtThings != null && buildingsList != null)
            {
                // Keep only buildings of this card's defs and count them. A load ID doesn't
                // name the stuff, so a card with madeFrom keeps its saved count
                builtThings.RemoveAll(id => !buildingsList.Any(def => IsBuildingOf(id, def)));
                if (madeFrom is null)
                    triggeredCount = builtThings.Count;
            }
        }

        public override bool UnlockOnStartup => triggeredCount >= count;

        public override bool Trigger(Building building)
        {
            if (buildingsList != null && buildingsList.Contains(building.def) && (madeFrom is null || madeFrom == building.Stuff))
            {
                if (builtThings.Contains(building.GetUniqueLoadID()))
                    return false;
                else
                    builtThings.Add(building.GetUniqueLoadID());
                triggeredCount++;
            }
            return triggeredCount >= count;
        }

        // A building's load ID is "Thing_" followed by its defName and ID number
        private static bool IsBuildingOf(string id, ThingDef def)
        {
            if (id is null || def is null)
                return false;
            string prefix = "Thing_" + def.defName;
            if (id.Length <= prefix.Length || !id.StartsWith(prefix, StringComparison.Ordinal))
                return false;
            for (int i = prefix.Length; i < id.Length; i++)
            {
                if (!char.IsDigit(id[i]))
                    return false;
            }
            return true;
        }

        List<ThingDef> buildingsList = new List<ThingDef>();

        protected List<string> builtThings;
    }
}
