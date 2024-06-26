﻿using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
	public class ItemCraftTracker : Tracker<Thing>
	{
		public ThingDef def;
		public ThingDef madeFrom;
		public QualityCategory? quality;
		public int count = 1;
        public ThingDef includeingredient;

        protected int triggeredCount;

		
        public override string Key
        {
            get { return "ItemCraftTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(QuestManager), nameof(QuestManager.Notify_ThingsProduced));
		public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.ThingSpawned));
		protected override string[] DebugText => new string[] { $"Def: {def?.defName ?? "None"}", 
																$"MadeFrom: {madeFrom?.defName ?? "Any"}",
                                                                $"includeingredient: {includeingredient?.defName ?? "Any"}",
                                                                $"Quality: {quality}", 
																$"Count: {count}", 
																$"Current: {triggeredCount}" };
		public ItemCraftTracker()
		{
		}

		public ItemCraftTracker(ItemCraftTracker reference) : base(reference)
		{
			def = reference.def;
			madeFrom = reference.madeFrom;
			quality = reference.quality;
			count = reference.count;
			triggeredCount = 0;
            includeingredient = reference.includeingredient;
        }

		public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref def, "def");
			Scribe_Defs.Look(ref madeFrom, "madeFrom");
			Scribe_Values.Look(ref quality, "quality");
			Scribe_Values.Look(ref count, "count", 1);
			Scribe_Values.Look(ref triggeredCount, "triggeredCount");
            Scribe_Defs.Look(ref includeingredient, "includeingredient");
        }

		public override bool Trigger(Thing thing)
		{
			base.Trigger(thing);
			

            if ((def is null || thing.def == def) && (madeFrom is null || madeFrom == thing.Stuff))
            {

                CompIngredients comping = thing.TryGetComp<CompIngredients>();

                if (includeingredient is null || (comping != null && comping.ingredients.Contains(includeingredient)))
                {
                    if (quality is null || (thing.TryGetQuality(out var qc) && qc >= quality))
                    {
                        triggeredCount = triggeredCount + thing.stackCount;

                    }
                }
            }

            return triggeredCount >= count;
		}
	}
}
