namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondUpdateMaximumOccupancyPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondUpdateMaximumOccupancyPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondUpdateMaximumOccupancyPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Fix for Professions compatibility.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.Professions")]
    private static void FishPondUpdateMaximumOccupancyPrefix(FishPond __instance, ref int __state)
    {
        __state = __instance.FishCount;
    }

    /// <summary>Set Tui-La pond capacity.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Professions")]
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance, int __state)
    {
        if (__instance.fishType.Value is "MNF.MoreNewFish_tui" or "MNF.MoreNewFish_la")
        {
            __instance.maxOccupants.Set(2);
        }

        if (__instance.FishCount >= __state)
        {
            return;
        }

        var fish = __instance.ParsePondFishes();
        fish.SortDescending();
        fish = fish.Take(__instance.FishCount).ToList();
        Data.Write(__instance, DataKeys.PondFish, string.Join(';', fish));
    }

    #endregion harmony patches
}
