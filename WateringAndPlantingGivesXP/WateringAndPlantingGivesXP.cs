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
using StardewValley.Objects;

namespace WateringAndPlantingGivesXP
{

    /// <summary>The mod entry point.</summary>
    public class WateringAndPlantingGivesXP : Mod
    {

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            //Patches for watering crops
            harmony.Patch(
               original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
               prefix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.GetWaterAmountBeforeWatering))
               );
            harmony.Patch(
               original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
               postfix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.Water))
               );

            //Patches for planting crops
            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.tryToPlaceItem)),
               prefix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.getFarmerForItem))
               );
            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.tryToPlaceItem)),
               postfix: new HarmonyMethod(typeof(WateringAndPlantingGivesXP), nameof(WateringAndPlantingGivesXP.Plant))
               );
        }


        /*********
        ** Private methods
        *********/
        //prefix function for getting water amount before watering
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
        
        //postfix function for after watering your crops
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

        //prefix function for before planting a crop
        private static bool getFarmerForItem(ref object[] __state, ref Item item)
        {
            if(item == null || item is Tool || item is Furniture || item is Wallpaper)
            {
                __state = new object[]
                {
                    null
                };
            } else if(item.Category == -19 || item.Category == -74) //category for crops
            {
                __state = new object[]
                {
                    Game1.player
                };
            } else
            {
                __state = new object[]
                {
                    null
                };
            }
            return true;
        }

        //postfix function for after planting a crop
        private static void Plant(object[] __state, bool __result)
        {
            if(__result)
            {
                if(__state[0] is Farmer)
                {
                    Farmer farmer = (Farmer)__state[0];
                    farmer.gainExperience(0, 10);
                }
            }
        }
    }
}
