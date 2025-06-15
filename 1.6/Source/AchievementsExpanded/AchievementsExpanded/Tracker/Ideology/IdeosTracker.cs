using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
	public class IdeosTracker : TrackerBase
	{
		
		public int count = 1;       
		
		protected int triggeredCount;

		
        public override string Key
        {
            get { return "IdeosTracker"; }
            set { }
        }

        public override Func<bool> AttachToLongTick => () => { return Trigger(); };
		protected override string[] DebugText => new string[] { $"Count: {count}", $"Current: {triggeredCount}" };

        public IdeosTracker()
		{
		}

		public IdeosTracker(IdeosTracker reference) : base(reference)
		{
			
			count = reference.count;  
			triggeredCount = 0;
        }

		public override bool UnlockOnStartup => Trigger();


		public override void ExposeData()
		{
			base.ExposeData();
		
			Scribe_Values.Look(ref count, "count", 1);
         
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
        }

		public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

		public override bool Trigger()
		{
			base.Trigger();
			if (Find.IdeoManager.classicMode)
			{
				return false;
			}
			if(Faction.OfPlayer != null) { triggeredCount = (Faction.OfPlayer.ideos.AllIdeos.Count()); }
			
            if (triggeredCount>= count)
			{
				return true;
			}
            return false;


        }
	}
}
