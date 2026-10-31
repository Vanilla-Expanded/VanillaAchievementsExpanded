using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Security.Cryptography;

namespace AchievementsExpanded
{
    public class NociosphereTracker : TrackerBase
    {
       

        public override string Key
        {
            get { return "NociosphereTracker"; }
            set { }
        }

        public int nociosphereActivations = 1;
        protected int triggeredCount;

        public override MethodInfo MethodHook => AccessTools.Method(typeof(CompNociosphere), nameof(CompNociosphere.OnActivityActivated));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.NociosphereActivated));
        protected override string[] DebugText => new string[] { $"Nociosphere activations:  {nociosphereActivations}" , $"Current: {triggeredCount}" };
    

        public override (float percent, string text) PercentComplete => nociosphereActivations > 1 ? ((float)triggeredCount / nociosphereActivations, $"{triggeredCount} / {nociosphereActivations}") : base.PercentComplete;


        public NociosphereTracker()
        {
        }


        public NociosphereTracker(NociosphereTracker reference) : base(reference)
        {
            nociosphereActivations = reference.nociosphereActivations;
            if (nociosphereActivations <= 0)
                nociosphereActivations = 1;
            triggeredCount = 0;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
            Scribe_Values.Look(ref nociosphereActivations, "nociosphereActivations", 0);
        }

        public override bool Trigger()
        {
            base.Trigger();
            triggeredCount++;
            if (triggeredCount > nociosphereActivations)
            {
                return true;
            }
            return false;

        }

        


    }
}
