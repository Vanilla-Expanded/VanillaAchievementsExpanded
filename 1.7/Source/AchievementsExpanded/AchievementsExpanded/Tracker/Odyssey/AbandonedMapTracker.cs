using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld.Planet;

namespace AchievementsExpanded
{
    public class AbandonedMapTracker : Tracker<bool>
    {
        public override string Key
        {
            get { return "AbandonedMapTracker"; }
            set { }
        }

        public int count;

        protected int triggeredCount;

        public override MethodInfo MethodHook => AccessTools.Method(typeof(MapParent), nameof(MapParent.Abandon));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.AbandonMap));
        protected override string[] DebugText => new string[] { $"Count: {count}", $"Current: {triggeredCount}" };

        public override (float percent, string text) PercentComplete => ((float)triggeredCount / count, $"{triggeredCount} / {count}");

        public AbandonedMapTracker()
        {
        }

        public AbandonedMapTracker(AbandonedMapTracker reference) : base(reference)
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
        }

        public override bool Trigger(bool wasGravshipLaunch)
        {
            base.Trigger();
            if (wasGravshipLaunch)
            {
                triggeredCount++;
            }          
            if (triggeredCount >= count)
            {
                return true;
            }
            return false;

        }
    }
}
