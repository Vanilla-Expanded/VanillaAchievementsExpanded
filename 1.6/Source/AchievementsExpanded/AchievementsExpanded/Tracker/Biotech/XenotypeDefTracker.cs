using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using HarmonyLib;

namespace AchievementsExpanded
{
	public class XenotypeDefTracker : PawnJoinedTracker
	{
		Dictionary<XenotypeDef, int> xenotypeDefs = new Dictionary<XenotypeDef, int>();
		public bool countTemporary = false;

		protected override string[] DebugText
		{
			get
			{
				List<string> text = new List<string>();
				foreach (var xenotype in xenotypeDefs)
				{
					string entry = $"Xenotype: {xenotype.Key?.defName ?? "None"} Count: {xenotype.Value}";
					text.Add(entry);
				}
				text.Add($"Require all in list: {requireAll}");
				return text.ToArray();
			}
		}

		public XenotypeDefTracker()
		{
		}

		public XenotypeDefTracker(XenotypeDefTracker reference) : base(reference)
		{
            xenotypeDefs = reference.xenotypeDefs;
			if (xenotypeDefs.EnumerableNullOrEmpty())
				Log.Error($"xenotypeDefs list for XenotypeDefTracker cannot be Null or Empty");
            countTemporary = reference.countTemporary;
        }

		public override bool UnlockOnStartup => Trigger(null);

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref xenotypeDefs, "xenotypeDefs", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref countTemporary, "countTemporary", false);
        }

		public override bool Trigger(Pawn param)
		{
			base.Trigger(param);
			bool trigger = true;
			XenotypeDef xenotypeDef = param?.genes?.Xenotype;
			List<Pawn> factionPawns;
            if (!countTemporary)
			{
                 factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
            }
			else {
                 factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
            }

                
			if (factionPawns is null)
				return false;
			foreach (KeyValuePair<XenotypeDef, int> set in xenotypeDefs)
			{
				var count = 0;
				if (set.Key == xenotypeDef)
					
					count += 1;
				if (requireAll)
				{
					if (factionPawns.Where(f => f.genes?.Xenotype?.defName == set.Key.defName).Count() + count < set.Value)
					{
						trigger = false;
					}
				}
				else
				{
					trigger = false;
					if (factionPawns.Where(f => f.genes?.Xenotype?.defName == set.Key.defName).Count() + count >= set.Value)
					{
						return true;
					}
				}
			}
			return trigger;
		}
	}
}
