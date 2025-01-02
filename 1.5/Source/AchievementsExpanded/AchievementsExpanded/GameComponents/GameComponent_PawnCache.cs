using RimWorld;
using RimWorld.Planet;
using Verse;
using RimWorld.QuestGen;


namespace AchievementsExpanded
{


    public class GameComponent_PawnCache : GameComponent
    {
        public int tickCounter = 10000;

        public GameComponent_PawnCache(Game game) : base()
        {
        }

        public override void GameComponentTick()
        {

            if (tickCounter > 10000)
            {

                foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_OfPlayerFaction)
                {
                    StaticCollections.AddPawnToList(pawn);
                }

                tickCounter = 0;

            }
            tickCounter++;
        }

    }

}

