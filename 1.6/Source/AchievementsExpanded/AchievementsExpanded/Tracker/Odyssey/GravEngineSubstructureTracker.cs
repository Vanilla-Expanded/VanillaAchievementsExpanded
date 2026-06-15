using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace AchievementsExpanded
{
    public class GravEngineSubstructureTracker : TrackerBase
    {
        public override string Key
        {
            get { return "GravEngineSubstructureTracker"; }
            set { }
        }

        public int count;

        protected int triggeredCount;

        public override Func<bool> AttachToDailyTick => () => { return Trigger(); };

        protected override string[] DebugText => new string[] { $"Count: {count}", $"Current: {triggeredCount}" };

        public override (float percent, string text) PercentComplete => ((float)triggeredCount / count, $"{triggeredCount} / {count}");

        public GravEngineSubstructureTracker()
        {
        }

        public GravEngineSubstructureTracker(GravEngineSubstructureTracker reference) : base(reference)
        {
            count = reference.count;
            if (count <= 0)
                count = 1;
            triggeredCount = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);

        }

        public override bool Trigger()
        {
            base.Trigger();

           

            foreach (Thing item in ReturnAllGravEngines(Find.CurrentMap.listerThings))
            {
                Building_GravEngine obj = item as Building_GravEngine;
                triggeredCount = obj.AllConnectedSubstructure.Count;
                
            }

            if (triggeredCount >= count)
            {
                return true;
            }
            return false;

        }

        private static List<Thing> ReturnAllGravEngines(ListerThings lister)
        {
            List<Thing> validEngines = new List<Thing>();

            validEngines.AddRange(lister.ThingsOfDef(ThingDefOf.GravEngine));
            if (InternalDefOf.VGE_GravjumperEngine != null)
            {
                validEngines.AddRange(lister.ThingsOfDef(InternalDefOf.VGE_GravjumperEngine));
            }
            if (InternalDefOf.VGE_GravhulkEngine != null)
            {
                validEngines.AddRange(lister.ThingsOfDef(InternalDefOf.VGE_GravhulkEngine));
            }
          
            return validEngines;
        }

    }
}
