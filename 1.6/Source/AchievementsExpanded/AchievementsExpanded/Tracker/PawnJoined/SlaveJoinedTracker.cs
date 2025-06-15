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
	public class SlaveJoinedTracker : TrackerBase
	{
		public int numberOfSlaves;
        protected int triggeredCount;


        public override string Key
        {
            get { return "SlaveJoinedTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus));
		public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.SlaveJoined));
        protected override string[] DebugText => new string[] { $"Number Of Slaves:  {numberOfSlaves}", $"Current: {triggeredCount}" };


        public override (float percent, string text) PercentComplete => numberOfSlaves > 1 ? ((float)triggeredCount / numberOfSlaves, $"{triggeredCount} / {numberOfSlaves}") : base.PercentComplete;

        public SlaveJoinedTracker()
		{
		}

		public SlaveJoinedTracker(SlaveJoinedTracker reference) : base(reference)
		{
            numberOfSlaves = reference.numberOfSlaves;
            if (numberOfSlaves <= 0)
                numberOfSlaves = 1;
            triggeredCount = 0;

        }

		public override bool UnlockOnStartup => Trigger();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref numberOfSlaves, "numberOfSlaves", 0);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
        }

        public override bool Trigger()
        {
            base.Trigger();
           
            ThingDef raceDef = ThingDefOf.Human;
            List<Pawn> factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_SlavesOfColony;
            
            if (factionPawns is null)
                return false;

            triggeredCount = factionPawns.Count();
            if (triggeredCount >= numberOfSlaves)
            {
                return true;
            }
            return false;
        }
    }
}
