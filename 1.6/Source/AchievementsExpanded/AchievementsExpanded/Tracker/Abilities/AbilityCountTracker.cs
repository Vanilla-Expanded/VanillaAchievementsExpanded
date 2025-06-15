using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Security.Cryptography;
using System.Linq;

namespace AchievementsExpanded
{
    public class AbilityCountTracker : Tracker2<AbilityDef, Pawn>
    {

        public int count;
        public AbilityDef abilityDef;
        public List<AbilityDef> abilityDefs;
        public bool onlyPsycast = false;
        public bool total = false;
        public bool countTemporary=false;

        [Unsaved]
        protected float triggeredCount;

        public override string Key
        {
            get { return "AbilityCountTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_AbilityTracker), "GainAbility");
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.AbilityGained));

        protected override string[] DebugText => new string[] { $"AbilityDef: {abilityDef?.defName ?? "None"}",
                                                                $"AbilityDefs: {abilityDefs?.ToStringSafeEnumerable() ?? "None"}",
                                                                $"total: {total}",
                                                                $"onlyPsycast: {onlyPsycast}",
                                                                $"countTemporary: {countTemporary}",
                                                                $"Count: {count}", $"Current: {triggeredCount}" };

        public AbilityCountTracker()
        {
        }

        public AbilityCountTracker(AbilityCountTracker reference) : base(reference)
        {

            count = reference.count;

            abilityDef = reference.abilityDef;
            abilityDefs = reference.abilityDefs;
            onlyPsycast = reference.onlyPsycast;
            countTemporary = reference.countTemporary;

            total = reference.total;
            if (count <= 0)
                count = 1;

            triggeredCount = 0;
        }



        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Defs.Look(ref abilityDef, "abilityDef");
            Scribe_Collections.Look(ref abilityDefs, "abilityDefs", LookMode.Def);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
            Scribe_Values.Look(ref onlyPsycast, "onlyPsycast", false);
            Scribe_Values.Look(ref total, "total");
            Scribe_Values.Look(ref countTemporary, "countTemporary", false);
        }

        public override (float percent, string text) PercentComplete => count > 1 ? (triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public override bool Trigger(AbilityDef ability, Pawn pawn)
        {
            if (!total)
            {
                if (pawn is null || pawn.Faction != Faction.OfPlayerSilentFail)
                {
                    return false;
                }                

                List<Ability> pawnAbilities = pawn.abilities.abilities;
                List<AbilityDef> pawnAbilitiesToSelect = pawnAbilities.Select(x => x.def).ToList();

                if (abilityDef != null)
                {
                    pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => x == abilityDef).ToList();
                }
                if (abilityDefs != null)
                {
                    pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => abilityDefs.Contains(x)).ToList();
                }
                if (onlyPsycast)
                {
                    pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => x.IsPsycast).ToList();
                }

                return pawnAbilitiesToSelect.Count() >= count;

            }

            else
            {
                triggeredCount = 0;
                List<Pawn> listOfPawns;
                if (!countTemporary)
                {
                    listOfPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
                }
                else
                {
                    listOfPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction;
                }
                if (listOfPawns is null)
                    return false;
                foreach (Pawn pawn2 in listOfPawns)
                {

                    List<Ability> pawnAbilities = pawn2.abilities.abilities;
                    List<AbilityDef> pawnAbilitiesToSelect = pawnAbilities.Select(x => x.def).ToList();

                    if (abilityDef != null)
                    {
                        pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => x == abilityDef).ToList();
                    }
                    if (abilityDefs != null)
                    {
                        pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => abilityDefs.Contains(x)).ToList();
                    }
                    if (onlyPsycast)
                    {
                        pawnAbilitiesToSelect = pawnAbilitiesToSelect.Where(x => x.IsPsycast).ToList();
                    }


                    triggeredCount += pawnAbilitiesToSelect.Count();
                    if (triggeredCount >= count)
                        return true;
                }
                if (triggeredCount >= count)
                    return true;
                return false;
            }
          








        }
    }
}
