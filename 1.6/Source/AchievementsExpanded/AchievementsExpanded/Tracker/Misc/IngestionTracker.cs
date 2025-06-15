using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AchievementsExpanded
{
    public class IngestionTracker : Tracker2<Thing, Pawn>
    {
        
        public float count = 1;
        public ThingDef ingestorThingDef;
        public List<ThingDef> ingestorsThingDefs;
        public ThingDef foodDef;
        public bool checkIfCorpse = false;
        public bool checkIfTree = false;
        public bool onlyCountAnimals = false;
        public bool onlyPlayerFaction = true;
        public QualityCategory? quality;      
        public ThingDef includeIngredientDef;

        [Unsaved]
        protected float triggeredCount;

        public override string Key
        {
            get { return "IngestionTracker"; }
            set { }
        }
        public override MethodInfo MethodHook => AccessTools.Method(typeof(Thing), nameof(Thing.Ingested)); 
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.IngestedThing));
      
        protected override string[] DebugText => new string[] { $"ingestorThingDef: {ingestorThingDef?.defName ?? "None"}",
                                                                $"ingestorsThingDefs: {ingestorsThingDefs?.ToStringSafeEnumerable() ?? "None"}",
                                                                $"foodDef: {foodDef?.defName ?? "None"}",
                                                                 $"includeIngredientDef: {includeIngredientDef?.defName ?? "None"}",
                                                                $"checkIfCorpse: {checkIfCorpse}",
                                                                 $"checkIfTree: {checkIfTree}",
                                                                  $"onlyCountAnimals: {onlyCountAnimals}",
                                                                   $"onlyPlayerFaction: {onlyPlayerFaction}",
                                                                $"Count: {count}", $"Current: {triggeredCount}" };


        public IngestionTracker()
        {
        }

        public IngestionTracker(IngestionTracker reference) : base(reference)
        {
            ingestorThingDef = reference.ingestorThingDef;
            ingestorsThingDefs = reference.ingestorsThingDefs;
            foodDef = reference.foodDef;
            checkIfCorpse = reference.checkIfCorpse;
            checkIfTree = reference.checkIfTree;
            onlyPlayerFaction = reference.onlyPlayerFaction;
            onlyCountAnimals = reference.onlyCountAnimals;
            quality = reference.quality;
            includeIngredientDef = reference.includeIngredientDef;
            count = reference.count;
            if (count <= 0)
                count = 1;
          
            triggeredCount = 0;
        }

       

        public override void ExposeData()
        {
            base.ExposeData();
         
            Scribe_Values.Look(ref count, "count", 1);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
            Scribe_Defs.Look(ref ingestorThingDef, "ingestorThingDef");
            Scribe_Collections.Look(ref ingestorsThingDefs, "ingestorsThingDefs", LookMode.Def);
            Scribe_Defs.Look(ref foodDef, "foodDef");
            Scribe_Values.Look(ref checkIfCorpse, "checkIfCorpse", false);
            Scribe_Values.Look(ref checkIfTree, "checkIfTree", false);
            Scribe_Values.Look(ref onlyPlayerFaction, "onlyPlayerFaction", true);
            Scribe_Values.Look(ref onlyCountAnimals, "onlyCountAnimals", false);
            Scribe_Values.Look(ref quality, "quality");         
            Scribe_Defs.Look(ref includeIngredientDef, "includeIngredientDef");

        }

        public override (float percent, string text) PercentComplete =>  count > 1 ? (triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public override bool Trigger(Thing thingIngested, Pawn ingester)
        {

            if(onlyPlayerFaction && ingester.Faction != Faction.OfPlayerSilentFail)
            {
                return false;
            }

            if (onlyCountAnimals && ingester.RaceProps?.Humanlike==true)
            {
                return false;
            }

            bool ingestorRace = ingestorThingDef is null || ingester.def == ingestorThingDef;
            bool ingestorRaces = ingestorsThingDefs.NullOrEmpty() || ingestorsThingDefs.Contains(ingester.def);
            bool food = foodDef is null || thingIngested.def == foodDef;
            bool corpse = !checkIfCorpse || thingIngested as Corpse != null;
            bool tree = !checkIfTree || thingIngested.def?.plant?.IsTree == true;
            bool foodQuality = quality is null || thingIngested.TryGetComp<CompQuality>()?.Quality >= quality;
            bool includeIngredient = includeIngredientDef is null || thingIngested.TryGetComp<CompIngredients>()?.ingredients?.Contains(includeIngredientDef)==true;

            return ingestorRace && ingestorRaces && food && tree && corpse && foodQuality && includeIngredient && (count <= 1 || ++triggeredCount >= count);

        }
    }
}
