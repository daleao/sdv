﻿namespace DaLion.Professions.Framework.Patchers.Fishing;

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

    /// <summary>Patch for Aquarist increased Fish Pond capacity.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.Ponds")]
    [UsedImplicitly]
    private static void FishPondUpdateMaximumOccupancyPostfix(
        FishPond __instance, FishPondData? ____fishPondData)
    {
        if (____fishPondData is null || !__instance.HasUnlockedFinalPopulationGate())
        {
            return;
        }

        if (__instance.fishType.Value is "MNF.MoreNewFish_tui" or "MNF.MoreNewFish_la")
        {
            __instance.maxOccupants.Set(2);
            __instance.currentOccupants.Set(Math.Min(__instance.currentOccupants.Value, __instance.maxOccupants.Value));
        }

        var owner = __instance.GetOwner();
        var occupancy = 10;
        var isLegendaryPond = __instance.GetFishObject().IsBossFish();
        if (owner.HasProfessionOrLax(Profession.Aquarist))
        {
            if (!isLegendaryPond)
            {
                occupancy += 2;
                if (owner.HasProfession(Profession.Aquarist, true))
                {
                    occupancy += 2;
                }
            }
            else if (!owner.HasProfession(Profession.Aquarist, true))
            {
                occupancy /= 2;
            }
        }

        __instance.maxOccupants.Set(occupancy);
        __instance.currentOccupants.Set(Math.Min(__instance.currentOccupants.Value, __instance.maxOccupants.Value));
    }

    #endregion harmony patches
}
