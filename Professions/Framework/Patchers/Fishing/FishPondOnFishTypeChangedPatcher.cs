namespace DaLion.Professions.Framework.Patchers.Fishing;

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

        Data.Write(__instance, DataKeys.FamilyLivingHere, null);
    }

    #endregion harmony patches
}
