namespace DaLion.Ponds.Framework.Patchers;

#region using directives

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

    /// <summary>Set Tui-La pond capacity.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("DaLion.Professions")]
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value is "MNF.MoreNewFish_tui" or "MNF.MoreNewFish_la")
        {
            __instance.maxOccupants.Set(2);
        }
    }

    #endregion harmony patches
}
