using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;

namespace AchievementsExpanded
{
	public class KindDefTracker : PawnJoinedTracker
	{
		Dictionary<PawnKindDef, int> kindDefs = new Dictionary<PawnKindDef, int>();
        public bool countTemporary = true;
        public bool countOnlySlaves = false;
        public bool countClones = false;

        protected override string[] DebugText
		{
			get
			{
				List<string> text = new List<string>();
				foreach (var kind in kindDefs)
				{
					string entry = $"Kind: {kind.Key?.defName ?? "None"} Count: {kind.Value}";
					text.Add(entry);
				}
				text.Add($"Require all in list: {requireAll}");
				return text.ToArray();
			}
		}

		public KindDefTracker()
		{
		}

		public KindDefTracker(KindDefTracker reference) : base(reference)
		{
			kindDefs = reference.kindDefs;
			if (kindDefs.EnumerableNullOrEmpty())
				Log.Error($"kindDefs list for KindDefTracker cannot be Null or Empty");
            countTemporary = reference.countTemporary;         
            countClones = reference.countClones;
        }

		public override bool UnlockOnStartup => Trigger(null);

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref kindDefs, "kindDefs", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref countTemporary, "countTemporary", true);         
            Scribe_Values.Look(ref countClones, "countClones", false);
        }

		public override bool Trigger(Pawn param)
		{
			base.Trigger(param);
			bool trigger = true;
			PawnKindDef kindDef = param?.kindDef;
            List<Pawn> factionPawns;
           
            if (!countTemporary)
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
            }
            else
            {
                factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }

            if (countClones)
            {
                factionPawns.RemoveWhere(x => !x.IsDuplicate);
            }

            if (factionPawns is null)
				return false;
			foreach (KeyValuePair<PawnKindDef, int> set in kindDefs)
			{
				var count = 0;
				if (set.Key == kindDef)
					count += 1;
				if (requireAll)
				{
					if (factionPawns.Where(f => f.kindDef.defName == set.Key.defName).Count() + count < set.Value)
					{
						trigger = false;
					}
				}
				else
				{
					trigger = false;
					if (factionPawns.Where(f => f.kindDef.defName == set.Key.defName).Count() + count >= set.Value)
					{
						return true;
					}
				}
			}
			return trigger;
		}
	}
}
