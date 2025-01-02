
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;


namespace AchievementsExpanded
{
    [StaticConstructorOnStartup]
    public static class StaticCollections
    {

        public static HashSet<Pawn> colonyPawns = new HashSet<Pawn>();

        public static void AddPawnToList(Pawn pawn)
        {

            if (!colonyPawns.Contains(pawn))
            {
                colonyPawns.Add(pawn);
            }
        }

    }
}
