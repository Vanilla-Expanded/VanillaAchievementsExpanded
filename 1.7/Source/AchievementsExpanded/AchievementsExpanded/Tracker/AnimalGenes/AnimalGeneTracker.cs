using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using VEF.AnimalGenes;

namespace AchievementsExpanded
{
    public class AnimalGeneTracker : TrackerBase
    {

        public override string Key
        {
            get { return "AnimalGeneTracker"; }
            set { }
        }

        Dictionary<AnimalGeneDef, int> animalGenesList = new Dictionary<AnimalGeneDef, int>();


        public override Func<bool> AttachToDailyTick => () => { return Trigger(); };
        protected override string[] DebugText
        {
            get
            {
                List<string> text = new List<string>();
                foreach (var gene in animalGenesList)
                {
                    string entry = $"Gene: {gene.Key?.defName ?? "None"} Count: {gene.Value}";
                    text.Add(entry);
                }
                text.Add($"Require all in list: true");
                return text.ToArray();
            }
        }


        public AnimalGeneTracker()
        {
        }




        public AnimalGeneTracker(AnimalGeneTracker reference) : base(reference)
        {
            animalGenesList = reference.animalGenesList;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref animalGenesList, "animalGenesList", LookMode.Def, LookMode.Value);

        }

        public override bool Trigger()
        {
            base.Trigger();

            bool trigger = true;
            List<Pawn> factionPawns;
            factionPawns = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead.Where(p => p.IsColonyAnimal).ToList();
            if (factionPawns.NullOrEmpty())
                return false;

            foreach (KeyValuePair<AnimalGeneDef, int> set in animalGenesList)
            {
                if (factionPawns.Where(p =>  UtilityMethods.HasAnimalGene(p, set.Key)).Count() < set.Value)
                {
                    trigger = false;
                }
            }

            return trigger;

        }

        public override bool UnlockOnStartup => Trigger();




    }
}
