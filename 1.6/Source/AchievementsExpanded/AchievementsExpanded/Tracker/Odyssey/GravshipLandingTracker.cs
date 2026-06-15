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
    public class GravshipLandingTracker : Tracker<Gravship>
    {
        public override string Key
        {
            get { return "GravshipLandingTracker"; }
            set { }
        }

        public int count;
        public int incidentCount;

        protected int triggeredCount;

        public override PatchType PatchType => PatchType.Prefix;

        public override MethodInfo MethodHook => AccessTools.Method(typeof(WorldComponent_GravshipController), "LandingEnded");
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.GravshipLanded));
        protected override string[] DebugText => new string[] { $"Count: {count}", $"incidentCount: {incidentCount}", $"Current: {triggeredCount}" };

        public override (float percent, string text) PercentComplete{
            get
            {
                if (count > 0)
                {
                    return ((float)triggeredCount / count, $"{triggeredCount} / {count}");
                }
                else if (incidentCount > 0) {
                    return ((float)triggeredCount / incidentCount, $"{triggeredCount} / {incidentCount}");

                }
                return base.PercentComplete;
            }
        }
        
        

        public GravshipLandingTracker()
        {
        }

        public GravshipLandingTracker(GravshipLandingTracker reference) : base(reference)
        {
            count = reference.count;
            incidentCount = reference.incidentCount;
            triggeredCount = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref count, "count", 0);
            Scribe_Values.Look(ref incidentCount, "incidentCount", 0);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount",0);
        }

        public override bool Trigger(Gravship gravship)
        {
            base.Trigger();
            if (count > 0)
            {
                triggeredCount++;
                if (triggeredCount >= count)
                {
                    return true;
                }
            }
            if (incidentCount > 0)
            {
                if (gravship?.Engine.launchInfo.doNegativeOutcome==true)
                {
                    triggeredCount++;
                }
                if (triggeredCount >= incidentCount)
                {
                    return true;
                }
            }
            
            return false;

        }
    }
}
