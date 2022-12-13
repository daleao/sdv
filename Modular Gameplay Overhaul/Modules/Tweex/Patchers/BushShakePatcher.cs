namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class BushShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BushShakePatcher"/> class.</summary>
    internal BushShakePatcher()
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
                  __instance.size.Value < Bush.greenTeaBush && TweexModule.Config.BerryBushesRewardExp;

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
