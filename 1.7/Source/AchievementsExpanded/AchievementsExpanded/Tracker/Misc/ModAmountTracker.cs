using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
	public class ModAmountTracker : TrackerBase
	{
		public int amountOfMods;
		

	

        public override string Key
        {
            get { return "ModAmountTracker"; }
            set { }
        }


        protected override string[] DebugText => new string[] { $"amountOfMods: {amountOfMods}"};
		public ModAmountTracker()
		{
		}

        public override (float percent, string text) PercentComplete => amountOfMods > 1 ? ((float)ModsConfig.ActiveModsInLoadOrder.Count() / amountOfMods, $"{ModsConfig.ActiveModsInLoadOrder.Count()} / {amountOfMods}") : base.PercentComplete;


        public ModAmountTracker(ModAmountTracker reference) : base(reference)
		{
            amountOfMods = reference.amountOfMods;
            if (amountOfMods <= 0)
                amountOfMods = 1;
        }

        public override bool UnlockOnStartup => Trigger();

        public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref amountOfMods, "amountOfMods");

		}

		public override bool Trigger()
		{
			

			if (ModsConfig.ActiveModsInLoadOrder.Count()>= amountOfMods)
			{
				
				return true;
			}
			return false;
		}
	}
}
