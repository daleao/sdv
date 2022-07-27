using StardewValley;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondUpdateMaximumOccupancyPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondUpdateMaximumOccupancyPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
    [HarmonyPostfix]
    private static void FishPondUpdateMaximumOccupancyPostfix(FishPond __instance,
        FishPondData? ____fishPondData)
    {
        if (__instance.IsLegendaryPond())
            __instance.maxOccupants.Set((int)ModEntry.Config.LegendaryPondPopulationCap);
        else if (____fishPondData is not null &&
                 (__instance.GetOwner().HasProfession(Profession.Aquarist) &&
                     __instance.HasUnlockedFinalPopulationGate() || ModEntry.Config.LaxOwnershipRequirements &&
                     Game1.game1.DoesAnyPlayerHaveProfession(Profession.Aquarist, out _)))
            __instance.maxOccupants.Set(12);
    }

    #endregion harmony patches
}