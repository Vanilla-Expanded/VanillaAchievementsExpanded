
using RimWorld;
using Verse;
namespace AchievementsExpanded
{
    public class RecordWorker_TimeInSpace : RecordWorker
    {
        public override bool ShouldMeasureTimeNow(Pawn pawn)
        {
            return pawn.Map?.BiomeAt(pawn.Position)?.inVacuum == true;
        }
    }
}
