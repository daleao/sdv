namespace DaLion.Overhaul.Modules.Ponds.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondOnFishTypeChangedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondOnFishTypeChangedPatcher"/> class.</summary>
    internal FishPondOnFishTypeChangedPatcher()
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

        __instance.Write(DataKeys.FishQualities, null);
        __instance.Write(DataKeys.FamilyQualities, null);
        __instance.Write(DataKeys.FamilyLivingHere, null);
        __instance.Write(DataKeys.DaysEmpty, 0.ToString());
        __instance.Write(DataKeys.SeaweedLivingHere, null);
        __instance.Write(DataKeys.GreenAlgaeLivingHere, null);
        __instance.Write(DataKeys.WhiteAlgaeLivingHere, null);
        __instance.Write(DataKeys.CheckedToday, null);
        __instance.Write(DataKeys.ItemsHeld, null);
    }

    #endregion harmony patches
}
