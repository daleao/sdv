namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal class BushShakePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BushShakePatch()
    {
        Original = RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Detects if the bush is ready for harvest.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool BushShakePrefix(Bush __instance, ref bool __state)
    {
        __state = __instance is not null && __instance.tileSheetOffset.Value == 1 && !__instance.townBush.Value &&
                  __instance.inBloom(Game1.GetSeasonForLocation(__instance.currentLocation), Game1.dayOfMonth) &&
                  __instance.size.Value < Bush.greenTeaBush && ModEntry.Config.BerryBushesRewardExp;

        return true; // run original logic
    }

    /// <summary>Adds foraging experience if the bush was harvested.</summary>
    [HarmonyPostfix]
    private static void BushShakePostfix(Bush __instance, bool __state)
    {
        if (__state && __instance.tileSheetOffset.Value == 0)
            Game1.player.gainExperience((int) SkillType.Foraging, 3);
    }

    #endregion harmony patches
}