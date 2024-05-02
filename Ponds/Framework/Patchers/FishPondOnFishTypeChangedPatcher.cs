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
    internal FishPondOnFishTypeChangedPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Reset Fish Pond data.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance, int old_value, int new_value)
    {
        if (old_value < 0 || new_value >= 0)
        {
            return;
        }

        Data.Write(__instance, DataKeys.FishQualities, null);
        Data.Write(__instance, DataKeys.FamilyQualities, null);
        Data.Write(__instance, DataKeys.DaysEmpty, 0.ToString());
        Data.Write(__instance, DataKeys.SeaweedLivingHere, null);
        Data.Write(__instance, DataKeys.GreenAlgaeLivingHere, null);
        Data.Write(__instance, DataKeys.WhiteAlgaeLivingHere, null);
        Data.Write(__instance, DataKeys.CheckedToday, null);
        Data.Write(__instance, DataKeys.ItemsHeld, null);
    }

    #endregion harmony patches
}
