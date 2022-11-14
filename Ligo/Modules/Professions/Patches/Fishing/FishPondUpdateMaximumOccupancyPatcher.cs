namespace DaLion.Ligo.Modules.Professions.Patches.Fishing;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondUpdateMaximumOccupancyPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondUpdateMaximumOccupancyPatcher"/> class.</summary>
    internal FishPondUpdateMaximumOccupancyPatcher()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.UpdateMaximumOccupancy));
    }

    #region harmony patches

    /// <summary>Patch for Aquarist increased max fish pond capacity.</summary>
    [HarmonyPostfix]
    private static void FishPondUpdateMaximumOccupancyPostfix(
        FishPond __instance, FishPondData? ____fishPondData)
    {
        if (__instance.HasLegendaryFish())
        {
            __instance.maxOccupants.Set((int)ModEntry.Config.Professions.LegendaryPondPopulationCap);
        }
        else if (____fishPondData is not null &&
                 ((__instance.GetOwner().HasProfession(Profession.Aquarist) &&
                   __instance.HasUnlockedFinalPopulationGate()) || (ModEntry.Config.Professions.LaxOwnershipRequirements &&
                                                                    Game1.game1.DoesAnyPlayerHaveProfession(
                                                                        Profession.Aquarist, out _))))
        {
            __instance.maxOccupants.Set(12);
        }
    }

    #endregion harmony patches
}
