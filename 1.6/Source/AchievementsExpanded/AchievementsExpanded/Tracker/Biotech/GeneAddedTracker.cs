using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
    public class GeneAddedTracker : TrackerBase
    {
      
        public override string Key
        {
            get { return "GeneAddedTracker"; }
            set { }
        }

        Dictionary<GeneDef, int> genesList = new Dictionary<GeneDef, int>();


        public override MethodInfo MethodHook => AccessTools.Method(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.AddGene), new Type[] { typeof(GeneDef), typeof(bool) });
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.GeneAdded));
        protected override string[] DebugText
        {
            get
            {
                List<string> text = new List<string>();
                foreach (var gene in genesList)
                {
                    string entry = $"Gene: {gene.Key?.defName ?? "None"} Count: {gene.Value}";
                    text.Add(entry);
                }
                text.Add($"Require all in list: true");
                return text.ToArray();
            }
        }


        public GeneAddedTracker()
        {
        }

       


        public GeneAddedTracker(GeneAddedTracker reference) : base(reference)
        {
            genesList = reference.genesList;
         
           

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref genesList, "genesList", LookMode.Def, LookMode.Value);
            
        }

        public override bool Trigger()
        {
            base.Trigger();
           
            bool trigger = true;
            List<Pawn> factionPawns;           
            factionPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoLodgers;
            if (factionPawns is null)
                return false;

            foreach (KeyValuePair<GeneDef, int> set in genesList)
            {
                if (factionPawns.Where(p => UtilityMethods.HasActiveGene(p, set.Key)).Count() < set.Value)
                {
                    trigger = false;
                }
            }

            return trigger;

        }

        public override bool UnlockOnStartup => Trigger();

      


    }
}
