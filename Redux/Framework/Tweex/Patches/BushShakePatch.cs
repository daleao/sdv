namespace DaLion.Redux.Framework.Tweex.Patches;

#region using directives

using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class BushShakePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BushShakePatch"/> class.</summary>
    internal BushShakePatch()
    {
        this.Target = this.RequireMethod<Bush>("shake");
    }

    #region harmony patches

    /// <summary>Detects if the bush is ready for harvest.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool BushShakePrefix(Bush __instance, ref bool __state)
    {
        __state = __instance.tileSheetOffset.Value == 1 && !__instance.townBush.Value &&
                  __instance.inBloom(Game1.GetSeasonForLocation(__instance.currentLocation), Game1.dayOfMonth) &&
                  __instance.size.Value < Bush.greenTeaBush && ModEntry.Config.Tweex.BerryBushesRewardExp;

        return true; // run original logic
    }

    /// <summary>Adds foraging experience if the bush was harvested.</summary>
    [HarmonyPostfix]
    private static void BushShakePostfix(Bush __instance, bool __state)
    {
        if (__state && __instance.tileSheetOffset.Value == 0)
        {
            Game1.player.gainExperience(Farmer.foragingSkill, 5);
        }
    }

    #endregion harmony patches
}
