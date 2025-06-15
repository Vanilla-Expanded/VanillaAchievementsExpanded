using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
	public class ItemTracker : TrackerBase
	{
		public ThingDef def;
		public int count = 1;
        Dictionary<ThingDef, int> thingList = new Dictionary<ThingDef, int>();
		public ThingCategoryDef category;
        public bool mustHaveAll = false;
     
        public bool considerQuality = false;
		public QualityCategory quality = QualityCategory.Normal;

        [Unsaved]
		protected int triggeredCount; //Only for display

		
        public override string Key
        {
            get { return "ItemTracker"; }
            set { }
        }

        public override Func<bool> AttachToLongTick => () => { return Trigger(); };
		

        protected override string[] DebugText => new string[] { $"Def: {def?.defName ?? "None"}",
                                                                $"thingList: {thingList?.ToStringSafeEnumerable() ?? "None"}",
                                                                $"category: {category?.defName ?? "Any"}",
                                                                $"mustHaveAll: {mustHaveAll}",
                                                              
                                                                $"Quality: {quality}",
                                                                $"Count: {count}",
                                                                $"Current: {triggeredCount}" };

        public ItemTracker()
		{
		}

		public ItemTracker(ItemTracker reference) : base(reference)
		{
			def = reference.def;
			count = reference.count;
            thingList = reference.thingList;
			mustHaveAll = reference.mustHaveAll;
			considerQuality = reference.considerQuality;
            quality =reference.quality;
            category = reference.category;
         

        }

        public override bool UnlockOnStartup => Trigger();

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref def, "def");
			Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref mustHaveAll, "mustHaveAll", false);
            Scribe_Values.Look(ref considerQuality, "considerQuality", false);
            Scribe_Values.Look(ref quality, "quality", QualityCategory.Normal);
            Scribe_Collections.Look(ref thingList, "thingList", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look(ref category, "category");
        

        }

        public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

		public override bool Trigger()
		{
			base.Trigger();
			if (thingList.Count>0) {
                bool playerHasIt = false;
                foreach (KeyValuePair<ThingDef, int> set in thingList)
                {
					
                    playerHasIt = UtilityMethods.PlayerHas(set.Key,category, considerQuality, quality,  out int total, count);
                                     
					if (mustHaveAll)
					{
                        if (!playerHasIt) { break; }
                    }
					else { if (playerHasIt) { break; } }
                    
                }
                return playerHasIt;

            } else { 
				return UtilityMethods.PlayerHas(def, category, considerQuality, quality,  out triggeredCount, count);
			}

			
		}
	}
}
