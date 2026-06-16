using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace AchievementsExpanded
{
    public enum GravshipPosition{

        Inside,
        Outside
    }

    public class GravshipLaunchContentsTracker : Tracker<Building_GravEngine>
    {
        public override string Key
        {
            get { return "GravshipLaunchContentsTracker"; }
            set { }
        }

        public int hostilesOnLiftoff =0;
        public int crewOnLiftoff = 0;
        public int animalsOnLiftoff = 0;
        public ThingDef specificAnimal = null;
        public List<ThingDef> itemsPresent = new List<ThingDef>();
        public GravshipPosition gravshipPosition = GravshipPosition.Inside;

        protected int triggeredCount;


        public override MethodInfo MethodHook => AccessTools.Method(typeof(WorldComponent_GravshipController), nameof(WorldComponent_GravshipController.InitiateTakeoff));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.GravshipTakeoff));
        protected override string[] DebugText => new string[] { };



        public GravshipLaunchContentsTracker()
        {
        }

        public GravshipLaunchContentsTracker(GravshipLaunchContentsTracker reference) : base(reference)
        {
            hostilesOnLiftoff = reference.hostilesOnLiftoff;
            crewOnLiftoff = reference.crewOnLiftoff;
            animalsOnLiftoff = reference.animalsOnLiftoff;
            itemsPresent = reference.itemsPresent;
            gravshipPosition = reference.gravshipPosition;
            specificAnimal = reference.specificAnimal;

            triggeredCount = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hostilesOnLiftoff, "hostilesOnLiftoff", 0);
            Scribe_Values.Look(ref crewOnLiftoff, "crewOnLiftoff", 0);
            Scribe_Values.Look(ref animalsOnLiftoff, "animalsOnLiftoff", 0);
            Scribe_Collections.Look(ref itemsPresent, "itemsPresent", LookMode.Def);
            Scribe_Values.Look(ref gravshipPosition, "gravshipPosition", GravshipPosition.Inside);
            Scribe_Defs.Look(ref specificAnimal, "specificAnimal");

            Scribe_Values.Look(ref triggeredCount, "triggeredCount", 0);

        }

        public override bool Trigger(Building_GravEngine engine)
        {
            base.Trigger();

            Map map = engine.Map;
            if (map != null) {

                bool inside = gravshipPosition == GravshipPosition.Inside;

                if (hostilesOnLiftoff != 0)
                {
                    
                    triggeredCount = engine.Map.mapPawns.AllPawns.Where(x => x.HostileTo(Faction.OfPlayer)&& engine.ValidSubstructureAt(x.PositionHeld) == inside).Count();
                    return triggeredCount >= hostilesOnLiftoff;
                }
                if (crewOnLiftoff != 0)
                {
                    triggeredCount = engine.Map.mapPawns.AllPawns.Where(x => x.Faction.IsPlayer && x.RaceProps.Humanlike && engine.ValidSubstructureAt(x.PositionHeld) == inside).Count();
                    return triggeredCount >= crewOnLiftoff;
                }
                if (animalsOnLiftoff != 0)
                {
                    triggeredCount = engine.Map.mapPawns.AllPawns.Where(x => x.Faction.IsPlayer && x.RaceProps.Animal && (specificAnimal is null || x.def == specificAnimal) && engine.ValidSubstructureAt(x.PositionHeld) == inside).Count();
                    return triggeredCount >= animalsOnLiftoff;
                }
                if (!itemsPresent.NullOrEmpty())
                {
                    bool itemsPresentFlag = false;
                    foreach(ThingDef item in itemsPresent)
                    {
                        List<Thing> allThingsOfDef = engine.Map.listerThings.ThingsOfDef(item);
                        if (allThingsOfDef.Count>0 && allThingsOfDef.Any(x => engine.ValidSubstructureAt(x.PositionHeld) == inside))
                        {
                            itemsPresentFlag = true;
                        }else itemsPresentFlag = false;

                    }

                    return itemsPresentFlag;
                }

            }

            return false;


        }
    }
}
