using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
    public class PlantTracker : Tracker<Plant>
    {

        public ThingDef def;
        public int count;
     



        protected int triggeredCount;

        protected HashSet<string> registeredPlants;

     
        public override string Key
        {
            get { return "PlantTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(Plant), nameof(Plant.PlantCollected));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.CheckPlantHarvested));
        protected override string[] DebugText => new string[] { $"Def: {def?.defName ?? "None"}", $"Count: {count}" };


        public PlantTracker()
        {
        }

        public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public PlantTracker(PlantTracker reference) : base(reference)
        {
            def = reference.def;
            count = reference.count;
            if (count <= 0)
                count = 1;
          

            registeredPlants = new HashSet<string>();
            triggeredCount = 0;


        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref count, "count", 1);
          

            Scribe_Collections.Look(ref registeredPlants, "registeredPlants");
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);

        }

        public override bool Trigger(Plant plant)
        {
            base.Trigger();
            if (def is null || def == plant.def)
            {
                
                    if (!registeredPlants.Add(plant.GetUniqueLoadID()))
                    {
                        return false;
                    }
                    triggeredCount++;
               
                
            }
            
            return triggeredCount >= count;
        }

        public override bool UnlockOnStartup => Trigger();



    }
}
