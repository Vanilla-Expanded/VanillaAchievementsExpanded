
using RimWorld;
using Verse;

namespace AchievementsExpanded
{
    [DefOf]
    public static class InternalDefOf
    {
        [MayRequire("vanillaexpanded.gravship")]
        public static ThingDef VGE_GravjumperEngine;

        [MayRequire("vanillaexpanded.gravship")]
        public static ThingDef VGE_GravhulkEngine;
    }
}
