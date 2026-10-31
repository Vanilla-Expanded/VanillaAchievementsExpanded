using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;

namespace AchievementsExpanded
{
	public class RaceDefTracker : PawnJoinedTracker
	{
		Dictionary<ThingDef, int> raceDefs = new Dictionary<ThingDef, int>();
		public bool countTemporary = true;
        public bool countOnlySlaves = false;
        public bool countClones = false;

        protected override string[] DebugText
		{
			get
			{
				List<string> text = new List<string>();
				foreach (var race in raceDefs)
				{
					string entry = $"Race: {race.Key?.defName ?? "None"} Count: {race.Value}";
					text.Add(entry);
				}
				text.Add($"Require all in list: {requireAll}");
				return text.ToArray();
			}
		}

		public RaceDefTracker()
		{
		}

		public RaceDefTracker(RaceDefTracker reference) : base(reference)
		{
			raceDefs = reference.raceDefs;
			if (raceDefs.EnumerableNullOrEmpty())
				Log.Error($"raceDefs list for RaceDefTracker cannot be Null or Empty");
            countTemporary = reference.countTemporary;
            countClones = reference.countClones;
        }

		public override bool UnlockOnStartup => Trigger(null);

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref raceDefs, "raceDefs", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref countTemporary, "countTemporary", true);
            Scribe_Values.Look(ref countClones, "countClones", false);
        }

		public override bool Trigger(Pawn param)
		{
			base.Trigger(param);
			bool trigger = true;
			ThingDef raceDef = param?.def;
			List<Pawn> factionPawns;
			
            if (!countTemporary)
			{
                 factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
            }
			else {
                 factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }

			if(countClones)
			{
				factionPawns.RemoveWhere(x => !x.IsDuplicate);
			}

            

            if (factionPawns is null)
				return false;
			foreach (KeyValuePair<ThingDef, int> set in raceDefs)
			{
				var count = 0;
				if (set.Key == raceDef)
					
					count += 1;
				if (requireAll)
				{
					if (factionPawns.Where(f => f.def.defName == set.Key.defName).Count() + count < set.Value)
					{
						trigger = false;
					}
				}
				else
				{
					trigger = false;
					if (factionPawns.Where(f => f.def.defName == set.Key.defName).Count() + count >= set.Value)
					{
						return true;
					}
				}
			}
			return trigger;
		}
	}
}
