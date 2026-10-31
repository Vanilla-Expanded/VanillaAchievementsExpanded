using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
    public class EntitiesInHoldTracker : Tracker<Map>
    {
     
        public override string Key
        {
            get { return "EntitiesInHoldTracker"; }
            set {  }
        }

        public int totalEntities = 0;
        Dictionary<ThingDef, int> entitiesList = new Dictionary<ThingDef, int>();
        protected int triggeredCount;


        public override MethodInfo MethodHook => AccessTools.Method(typeof(CompHoldingPlatformTarget), nameof(CompHoldingPlatformTarget.Notify_HeldOnPlatform));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.AddedEntityToPlatform));
        protected override string[] DebugText
        {
            get
            {
                List<string> text = new List<string>();
                foreach (var entity in entitiesList)
                {
                    string entry = $"Entity: {entity.Key?.defName ?? "None"} Count: {entity.Value}";
                    text.Add(entry);
                }
                text.Add($"Require all in list: true");
                return text.ToArray();
            }
        }


        public EntitiesInHoldTracker()
        {
        }


        public EntitiesInHoldTracker(EntitiesInHoldTracker reference) : base(reference)
        {
            totalEntities = reference.totalEntities;
            entitiesList = reference.entitiesList;
            triggeredCount = 0;

        }

        public override (float percent, string text) PercentComplete => totalEntities > 0 ? ((float)triggeredCount / totalEntities, $"{triggeredCount} / {totalEntities}") : base.PercentComplete;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref entitiesList, "entitiesList", LookMode.Def, LookMode.Value);
            Scribe_Values.Look(ref totalEntities, "totalEntities", 1);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);
        }

        public override bool Trigger(Map map)
        {
            base.Trigger(map);
           
            if (totalEntities > 0) {

                triggeredCount = 0;
                List<Thing> holdingStructures = map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder);
                foreach(Thing holdingStructure in holdingStructures)
                {
                    Building_HoldingPlatform holdingStructureWithClass = (Building_HoldingPlatform)holdingStructure;
                    if (holdingStructureWithClass?.HeldPawn != null)
                    {
                        triggeredCount++;
                    }
                }
                if (triggeredCount >= totalEntities) {
                    return true;
                }
                return false;
            }
            else
            {

                bool trigger = true;

                List<Thing> holdingStructures = map.listerThings.ThingsInGroup(ThingRequestGroup.EntityHolder);

                foreach (KeyValuePair<ThingDef, int> set in entitiesList)
                {
                    if (holdingStructures.Where(h => h.TryGetComp<CompEntityHolder>()?.HeldPawn?.def == set.Key).Count()< set.Value)
                    {
                        trigger = false;
                    }

                }

                return trigger;
            }


            

        }

        public override bool UnlockOnStartup => Trigger();




    }
}
