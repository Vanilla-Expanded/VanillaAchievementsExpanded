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
    public class RoyalTitleTracker : TrackerBase
    {
        public RoyalTitleDef title;
        public int count;
        public bool countTemporary = false;
        protected int triggeredCount;


        public override string Key
        {
            get { return "RoyalTitleTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_RoyaltyTracker), nameof(Pawn_RoyaltyTracker.SetTitle)); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.TitleSet));
        protected override string[] DebugText => new string[] { $"title: {title?.defName ?? "[NullDef]"}", $"Count: {count}", $"countTemporary: {countTemporary}" };

        public RoyalTitleTracker()
        {
        }

        public RoyalTitleTracker(RoyalTitleTracker reference) : base(reference)
        {
            title = reference.title;
            count = reference.count;
            if (count <= 0)
                count = 1;
            countTemporary = reference.countTemporary;
            triggeredCount = 0;

        }

        public override bool UnlockOnStartup
        {
            get
            {            
               return Trigger();              
            }
        }

        public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref title, "title");
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref countTemporary, "countTemporary", false);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
        }

        public override bool Trigger()
        {

            base.Trigger();

            List<Pawn> factionPawns;
            
            if (!countTemporary)
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
            }
            else
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }
            if (factionPawns is null)
                return false;

            triggeredCount = factionPawns.Where(f => f.royalty?.MainTitle()?.seniority >= title?.seniority).Count();

            if (triggeredCount >= count)
            {
                return true;
            }
            return false;


        }
    }
}
