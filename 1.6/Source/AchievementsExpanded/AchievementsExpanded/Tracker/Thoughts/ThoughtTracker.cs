using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AchievementsExpanded
{
    public class ThoughtTracker : Tracker<Thought_Memory>
    {
        public ThoughtDef def;
        public int stageIndex = 0;
       
        public bool countAllMapPawns = false;

        protected int triggeredCount;


        public override string Key
        {
            get { return "ThoughtTracker"; }
            set { }
        }

        public override MethodInfo MethodHook => AccessTools.Method(typeof(MemoryThoughtHandler), nameof(MemoryThoughtHandler.TryGainMemory), new[] { typeof(Thought_Memory), typeof(Pawn) });
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(AchievementHarmony), nameof(AchievementHarmony.ThoughtInitialized));
        protected override string[] DebugText => new string[] { $"Def: {def.defName}", $"Current: {triggeredCount}", $"stageIndex: {stageIndex}", $"countAllMapPawns: {countAllMapPawns}"

        };

        public ThoughtTracker()
        {
        }

        public ThoughtTracker(ThoughtTracker reference) : base(reference)
        {
            def = reference.def;
            stageIndex = reference.stageIndex;

            triggeredCount = reference.triggeredCount;
         
            countAllMapPawns = reference.countAllMapPawns;


        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref stageIndex, "stageIndex", 0);

            Scribe_Values.Look(ref countAllMapPawns, "countAllMapPawns", false);
            Scribe_Values.Look(ref triggeredCount, "triggeredCount");
        }

        

        public override bool Trigger(Thought_Memory thought)
        {
            if (thought?.pawn != null && (countAllMapPawns || (!countAllMapPawns && thought.pawn.Faction == Faction.OfPlayerSilentFail))
                && (stageIndex==0 || (stageIndex>0 && thought.CurStageIndex >= stageIndex))
                && (def is null || def == thought.def) )
            {
                return true;
            }
            return false;
        }
    }
}
