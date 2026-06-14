using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace AchievementsExpanded
{
    public class MapEnteringTracker : TrackerBase
    {
        public override string Key
        {
            get { return "MapEnteringTracker"; }
            set { }
        }

        public int count;

        protected int triggeredCount;

        public override MethodInfo MethodHook => AccessTools.Method(typeof(GetOrGenerateMapUtility), nameof(GetOrGenerateMapUtility.GetOrGenerateMap),
            new[] { typeof(PlanetTile), typeof(IntVec3), typeof(WorldObjectDef), typeof(IEnumerable<GenStepWithParams>), typeof(bool) });
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.MapEntered));
        protected override string[] DebugText => new string[] { $"Count: {count}", $"Current: {triggeredCount}" };

        public override (float percent, string text) PercentComplete => ((float)triggeredCount / count, $"{triggeredCount} / {count}");

        public MapEnteringTracker()
        {
        }

        public MapEnteringTracker(MapEnteringTracker reference) : base(reference)
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
