using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
    public class BirthsTracker : TrackerBase
    {
      
        public override string Key
        {
            get { return "BirthsTracker"; }
            set { }
        }

        public int count;
      
        protected int triggeredCount;

        public override MethodInfo MethodHook => AccessTools.Method(typeof(RitualOutcomeEffectWorker_ChildBirth), nameof(RitualOutcomeEffectWorker_ChildBirth.Apply));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.ColonistBirth));
        protected override string[] DebugText => new string[] {  $"Count: {count}", $"Current: {triggeredCount}" };

        public override (float percent, string text) PercentComplete => ((float)triggeredCount / count, $"{triggeredCount} / {count}");


        public BirthsTracker()
        {
        }


        public BirthsTracker(BirthsTracker reference) : base(reference)
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
