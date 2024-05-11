namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPonds;

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

    /// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.Ponds")]
    private static void FishPondUpdateMaximumOccupancyPostfix(
        FishPond __instance, FishPondData? ____fishPondData)
    {
        if (____fishPondData is null || !__instance.HasUnlockedFinalPopulationGate())
        {
            return;
        }

        var owner = __instance.GetOwner();
        var occupancy = 10;
        if (owner.HasProfessionOrLax(Profession.Aquarist))
        {
            occupancy += 2;
            if (owner.HasProfession(Profession.Aquarist, true))
            {
                occupancy += 2;
            }
        }

        if (__instance.GetFishObject().IsBossFish())
        {
            occupancy /= 2;
        }

        __instance.maxOccupants.Set(occupancy);
        __instance.currentOccupants.Set(Math.Min(__instance.currentOccupants.Value, __instance.maxOccupants.Value));
    }

    #endregion harmony patches
}
