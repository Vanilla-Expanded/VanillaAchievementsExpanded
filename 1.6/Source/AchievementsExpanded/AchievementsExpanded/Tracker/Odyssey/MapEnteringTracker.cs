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
    public class MapEnteringTracker : Tracker<Map>
    {
        public override string Key
        {
            get { return "MapEnteringTracker"; }
            set { }
        }

        public int simpleMapVisitCount;
        public List<BiomeDef> biomesToVisit = new List<BiomeDef>();
        public List<TileMutatorDef> mutatorsToVisit = new List<TileMutatorDef>();
        public List<LandmarkDef> landmarksToVisit = new List<LandmarkDef>();

        protected int triggeredCount;
        protected List<string> biomesVisited = new List<string>();
        protected List<string> mutatorsVisited = new List<string>();
        protected List<string> landmarksVisited = new List<string>();


        public override MethodInfo MethodHook => AccessTools.Method(typeof(GetOrGenerateMapUtility), nameof(GetOrGenerateMapUtility.GetOrGenerateMap),
            new[] { typeof(PlanetTile), typeof(IntVec3), typeof(WorldObjectDef), typeof(IEnumerable<GenStepWithParams>), typeof(bool) });
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.MapEntered));
        protected override string[] DebugText => new string[] { $"Count: {simpleMapVisitCount}", $"Current: {triggeredCount}" };


        public override (float percent, string text) PercentComplete => simpleMapVisitCount > 0 ? (triggeredCount / simpleMapVisitCount, $"{triggeredCount} / {simpleMapVisitCount}") : base.PercentComplete;


        public MapEnteringTracker()
        {
        }

        public MapEnteringTracker(MapEnteringTracker reference) : base(reference)
        {
            simpleMapVisitCount = reference.simpleMapVisitCount;
            biomesToVisit = reference.biomesToVisit;
            mutatorsToVisit = reference.mutatorsToVisit;
            landmarksToVisit = reference.landmarksToVisit;
            triggeredCount = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref simpleMapVisitCount, "count", 1);
            Scribe_Collections.Look(ref biomesVisited, "biomesVisited");
            Scribe_Collections.Look(ref mutatorsVisited, "mutatorsVisited");
            Scribe_Collections.Look(ref landmarksVisited, "landmarksVisited");
        }

        public override bool Trigger(Map map)
        {
            base.Trigger();

            var biome = map.TileInfo?.PrimaryBiome;
            var mutators = map.TileInfo?.Mutators;
            var landmark = map.TileInfo?.Landmark;
          
            bool biomeFlag = false;
            if (biomesToVisit.NullOrEmpty())
            {
                biomeFlag = true;
            }
            else if(biome!=null&&(biomesToVisit.Contains(biome) && !biomesVisited.Contains(biome.defName)))
            {
                biomesVisited.Add(biome.defName);
                biomeFlag = biomesToVisit.All(m => biomesVisited.Contains(m.defName));
            }

            bool mutatorsFlag = false;

            if (mutatorsToVisit.NullOrEmpty())
            {
                mutatorsFlag = true;
            }
            else if(!mutators.NullOrEmpty())
            {
                foreach (var mutator in mutators)
                {
                    if (mutator != null && mutatorsToVisit.Contains(mutator) && !mutatorsVisited.Contains(mutator.defName))
                    {
                        mutatorsVisited.Add(mutator.defName);
                    }
                }

                mutatorsFlag = mutatorsToVisit.All(m => mutatorsVisited.Contains(m.defName));
            }

            bool landmarkFlag = false;
            if (landmarksToVisit.NullOrEmpty())
            {
                landmarkFlag = true;
            }
            else if (landmark!=null&&(landmarksToVisit.Contains(landmark.def) && !landmarksVisited.Contains(landmark.def.defName)))
            {
                landmarksVisited.Add(landmark.def.defName);
                landmarkFlag = landmarksToVisit.All(m => landmarksVisited.Contains(m.defName));
            }

            bool simpleCount = simpleMapVisitCount > 0;

            if (simpleCount)
            {
                triggeredCount++;
                if (triggeredCount >= simpleMapVisitCount)
                {
                    return true;
                }
                return false;
            }
            
            return biomeFlag && mutatorsFlag && landmarkFlag;

        }
    }
}
