using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace AchievementsExpanded
{
    public class MechClusterDefeatTracker : TrackerBase
    {
       
        public int count;
        protected int triggeredCount;

        public override string Key
        {
            get { return "MechClusterDefeatTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Lord), nameof(Lord.Notify_MechClusterDefeated)); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.DefeatedMechCluster));
        protected override string[] DebugText => new string[] { $"Count: {count}", $"Current: {triggeredCount}" };

        public MechClusterDefeatTracker()
        {
        }

        public MechClusterDefeatTracker(MechClusterDefeatTracker reference) : base(reference)
        {
          
            count = reference.count;
            if (count <= 0)
                count = 1;
            triggeredCount = 0;

        }

        public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;


        public override void ExposeData()
        {
            base.ExposeData();
          
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);

        }

        public override bool Trigger()
        {

            base.Trigger();
            triggeredCount++;
            if (triggeredCount >= count)
            {
                return true;
            }
            return false;


        }
    }
}
