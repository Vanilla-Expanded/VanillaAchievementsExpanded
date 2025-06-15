using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace AchievementsExpanded
{
    public class VAESettings : ModSettings
    {
        public bool writeAllSettings;
        public bool usePointsSystem = true;
        public bool collateModTabs = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref usePointsSystem, "usePointsSystem", true, true);
            Scribe_Values.Look(ref collateModTabs, "collateModTabs", false, true);
        }
    }

    public class VAEMod : Mod
    {
        public static VAESettings settings;

        public VAEMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<VAESettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var listing = new Listing_Standard();

            listing.Begin(inRect);

            listing.CheckboxLabeled("VAE_UsePointsSystem".Translate(), ref settings.usePointsSystem, "VAE_UsePointsSystemDesc".Translate());

            listing.Gap(30f);
            listing.CheckboxLabeled("VAE_CollateModTabs".Translate(), ref settings.collateModTabs, "VAE_CollateModTabsDesc".Translate());

            listing.Gap(30f);
            if (Prefs.DevMode) {
                Rect buttonRect = new Rect(inRect)
                {
                    width = inRect.width / 5
                };

                //listing.ConfirmationBoxCheckboxLabeled("DebugWriter".Translate(), ref settings.writeAllSettings);
                if (listing.ButtonText("GenerateLogInfo".Translate(), "GenerateLogInfoTooltip".Translate()))
                {
                    DebugWriter.PushToFile();
                }
            }
            
            

            listing.End();
        }

        public override string SettingsCategory()
        {
            return UtilityMethods.BaseModActive ? "VAE".Translate().ToString() : string.Empty;
        }
    }
}
