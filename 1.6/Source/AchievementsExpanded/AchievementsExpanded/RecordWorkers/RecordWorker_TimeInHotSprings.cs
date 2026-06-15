
using RimWorld;
using Verse;
namespace AchievementsExpanded
{
    public class RecordWorker_TimeInHotSprings : RecordWorker
    {
        public override bool ShouldMeasureTimeNow(Pawn pawn)
        {
            if(pawn.Map == null) return false;
            return pawn.PositionHeld.GetTerrain(pawn.Map) == TerrainDefOf.HotSpring;
        }
    }
}
