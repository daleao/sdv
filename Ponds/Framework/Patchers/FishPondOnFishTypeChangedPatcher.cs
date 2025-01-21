namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondOnFishTypeChangedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondOnFishTypeChangedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondOnFishTypeChangedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Reset Fish Pond data.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance, string? old_value, string? new_value)
    {
        if (string.IsNullOrEmpty(old_value) || !string.IsNullOrWhiteSpace(new_value))
        {
            return;
        }

        Data.Write(__instance, DataKeys.PondFish, null);
        Data.Write(__instance, DataKeys.DaysEmpty, 0.ToString());
        Data.Write(__instance, DataKeys.CheckedToday, null);
        Data.Write(__instance, DataKeys.ItemsHeld, null);
    }

    #endregion harmony patches
}
