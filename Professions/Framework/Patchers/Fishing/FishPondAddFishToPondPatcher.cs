namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.IO;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[ModConflict("DaLion.Ponds")]
internal sealed class FishPondAddFishToPondPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondAddFishToPondPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishPondAddFishToPondPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishPond>("addFishToPond");
    }

    #region harmony patches

    /// <summary>Distinguish extended family pairs + increment total Fish Pond quality ratings.</summary>
    [HarmonyPostfix]
    private static void FishPondAddFishToPondPostfix(FishPond __instance, SObject fish)
    {
        try
        {
            if (fish.ItemId != __instance.fishType.Value && Lookups.LegendaryFishes.Contains(fish.QualifiedItemId))
            {
                Data.Increment(__instance, DataKeys.FamilyLivingHere);
            }
        }
        catch (InvalidDataException ex)
        {
            Log.W($"{ex}\nThe data will be reset.");
            Data.Write(__instance, DataKeys.FamilyLivingHere, null);
        }
    }

    #endregion harmony patches
}
