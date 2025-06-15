using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace AchievementsExpanded
{
    public class AbilitiesBeingUsedTracker : Tracker3<AbilityDef, Pawn, LocalTargetInfo>
    {

        public int count;
        public AbilityDef abilityDef;
        public List<AbilityDef> abilityDefs;
        public ThingDef casterThingDef;
        public ThingDef targetThingDef;
        public List<ThingDef> targetThingDefs;
        public bool checkTargetOnlyOfPlayerFaction = false;
        public TechLevel? targetFactionTechLevel;
        public bool onlyPsycast = false;
        public bool onlyPlayerFaction = true;
        public float targetBelowHealthPercentage;
        public bool targetOnFire = false;
        public bool targetManhunter = false;


        [Unsaved]
        protected float triggeredCount;

        public override string Key
        {
            get { return "AbilitiesBeingUsedTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Ability), "PreActivate"); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.AbilityActivated));
      
        protected override string[] DebugText => new string[] { $"AbilityDef: {abilityDef?.defName ?? "None"}",
                                                                $"AbilityDefs: {abilityDefs?.ToStringSafeEnumerable() ?? "None"}",
                                                                $"casterThingDef: {casterThingDef?.defName ?? "None"}",
                                                                $"casterThingDef: {targetThingDef?.defName ?? "None"}",
                                                                $"onlyPsycast: {onlyPsycast}",
                                                                $"checkTargetOnlyOfPlayerFaction: {checkTargetOnlyOfPlayerFaction}",
                                                                $"targetBelowHealthPercentage: {targetBelowHealthPercentage}",
                                                                $"targetOnFire: {targetOnFire}",
                                                                $"targetManhunter: {targetManhunter}",
                                                                $"onlyPlayerFaction: {onlyPlayerFaction}",
                                                                $"Count: {count}", $"Current: {triggeredCount}" };

        public AbilitiesBeingUsedTracker()
        {
        }

        public AbilitiesBeingUsedTracker(AbilitiesBeingUsedTracker reference) : base(reference)
        {
            
            count = reference.count;
            onlyPlayerFaction = reference.onlyPlayerFaction;
            abilityDef = reference.abilityDef;
            abilityDefs = reference.abilityDefs;
            casterThingDef = reference.casterThingDef;
            targetThingDef = reference.targetThingDef;
            targetThingDefs = reference.targetThingDefs;
            targetFactionTechLevel = reference.targetFactionTechLevel;
            onlyPsycast = reference.onlyPsycast;
            targetOnFire = reference.targetOnFire;
            targetBelowHealthPercentage = reference.targetBelowHealthPercentage;
            targetManhunter = reference.targetManhunter;
            checkTargetOnlyOfPlayerFaction = reference.checkTargetOnlyOfPlayerFaction;

            if (count <= 0)
                count = 1;

            triggeredCount = 0;
        }

       

        public override void ExposeData()
        {
            base.ExposeData();
         
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref onlyPlayerFaction, "onlyPlayerFaction", true);
            Scribe_Defs.Look(ref abilityDef, "abilityDef");
            Scribe_Collections.Look(ref abilityDefs, "abilityDefs", LookMode.Def);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
            Scribe_Defs.Look(ref casterThingDef, "casterThingDef");
            Scribe_Defs.Look(ref targetThingDef, "targetThingDef");
            Scribe_Collections.Look(ref targetThingDefs, "targetThingDefs", LookMode.Def);
            Scribe_Values.Look(ref onlyPsycast, "onlyPsycast", false);
            Scribe_Values.Look(ref targetBelowHealthPercentage, "targetBelowHealthPercentage", 0);
            Scribe_Values.Look(ref targetOnFire, "targetOnFire", false);
            Scribe_Values.Look(ref targetFactionTechLevel, "targetFactionTechLevel");
            Scribe_Values.Look(ref checkTargetOnlyOfPlayerFaction, "checkTargetOnlyOfPlayerFaction", false);
            Scribe_Values.Look(ref targetManhunter, "targetManhunter", false);
        }

        public override (float percent, string text) PercentComplete =>  count > 1 ? (triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public override bool Trigger(AbilityDef ability, Pawn caster, LocalTargetInfo target)
        {
            if (onlyPlayerFaction && caster.Faction != Faction.OfPlayerSilentFail)
            {
                return false;
            }

            if(onlyPsycast && !ability.IsPsycast) {

                return false;
            }
           
            bool abilityDetected = abilityDef is null || abilityDef == ability;
            bool abilitiesDetected = abilityDefs.NullOrEmpty() || abilityDefs.Contains(ability);
            bool belowHealth = targetBelowHealthPercentage != 0 || target.Pawn?.health.summaryHealth.SummaryHealthPercent< targetBelowHealthPercentage;
            bool casterDef = casterThingDef is null || casterThingDef == caster.def;
            bool targetDef = targetThingDef is null || targetThingDef == target.Pawn?.def;
            bool targetDefs = targetThingDefs.NullOrEmpty() || targetThingDefs.Contains(target.Pawn?.def);
            bool onFire = !targetOnFire || target.Pawn?.IsBurning()==true;
            bool techlevel = targetFactionTechLevel is null || target.Pawn?.Faction?.def?.techLevel == targetFactionTechLevel;
            bool targetFaction = !checkTargetOnlyOfPlayerFaction || target.Pawn?.Faction == Faction.OfPlayerSilentFail;
            bool manhunter = !targetManhunter || target.Pawn?.InAggroMentalState == true;

            return abilitiesDetected && abilityDetected && casterDef && targetFaction && targetDef && targetDefs && techlevel && belowHealth && onFire && manhunter && (count <= 1 || ++triggeredCount >= count);


        }
    }
}
