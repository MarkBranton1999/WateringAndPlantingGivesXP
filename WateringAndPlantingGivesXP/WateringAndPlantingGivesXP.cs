using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using HarmonyLib;
using System.Reflection;

namespace WateringAndPlantingGivesXP
{

    /// <summary>The mod entry point.</summary>
    public class WateringAndPlantingGivesXP : Mod
    {

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
               prefix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.GetWaterAmountBeforeWatering))
               );
            harmony.Patch(
               original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
               postfix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.Water))
               );

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

        private static bool GetWaterAmountBeforeWatering(ref object[] __state, ref Farmer who)
        {
            if (who.CurrentTool is WateringCan)
            {
                WateringCan can = (WateringCan)who.CurrentTool;
                __state = new object[]
                {
                    can.WaterLeft
                };
                
            }

            return true;
        }
        private static void Water(object[] __state, ref Farmer who)
        {
            if (who.CurrentTool is WateringCan)
            {
                int waterLeft = Convert.ToInt32(__state[0]);
                WateringCan can = (WateringCan)who.CurrentTool;
                if (can.WaterLeft < waterLeft)
                {
                    who.gainExperience(0, (int)(waterLeft - can.WaterLeft));
                }
            }
        }
    }
}
