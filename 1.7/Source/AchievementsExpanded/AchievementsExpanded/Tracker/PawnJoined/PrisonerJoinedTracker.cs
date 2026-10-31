using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;
using Verse.AI.Group;

namespace AchievementsExpanded
{
	public class PrisonerJoinedTracker : TrackerBase
	{
		public int numberOfPrisoners;
        protected int triggeredCount;


        public override string Key
        {
            get { return "PrisonerJoinedTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus));
		public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.PrisonerJoined));
        protected override string[] DebugText => new string[] { $"Number Of Prisoners:  {numberOfPrisoners}", $"Current: {triggeredCount}" };


        public override (float percent, string text) PercentComplete => numberOfPrisoners > 1 ? ((float)triggeredCount / numberOfPrisoners, $"{triggeredCount} / {numberOfPrisoners}") : base.PercentComplete;

        public PrisonerJoinedTracker()
		{
		}

		public PrisonerJoinedTracker(PrisonerJoinedTracker reference) : base(reference)
		{
            numberOfPrisoners = reference.numberOfPrisoners;
            if (numberOfPrisoners <= 0)
                numberOfPrisoners = 1;
            triggeredCount = 0;

        }

		public override bool UnlockOnStartup => Trigger();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref numberOfPrisoners, "numberOfPrisoners", 0);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
        }

        public override bool Trigger()
        {
            base.Trigger();
           
            ThingDef raceDef = ThingDefOf.Human;
            List<Pawn> factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_PrisonersOfColony;
            
            if (factionPawns is null)
                return false;

            triggeredCount = factionPawns.Count();
            if (triggeredCount >= numberOfPrisoners)
            {
                return true;
            }
            return false;
        }
    }
}
