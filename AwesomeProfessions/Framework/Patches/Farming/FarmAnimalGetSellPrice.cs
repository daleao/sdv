using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Farming;

[UsedImplicitly]
internal class FarmAnimalGetSellPricePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalGetSellPricePatch()
    {
        Original = RequireMethod<FarmAnimal>(nameof(FarmAnimal.getSellPrice));
    }

    #region harmony patches

    /// <summary>Patch to adjust Breeder animal sell price.</summary>
    [HarmonyPrefix]
    private static bool FarmAnimalGetSellPricePrefix(FarmAnimal __instance, ref int __result)
    {
        double adjustedFriendship;
        try
        {
            var owner = Game1.getFarmerMaybeOffline(__instance.ownerID.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession("Breeder")) return true; // run original logic

            adjustedFriendship = Utility.Professions.GetProducerAdjustedFriendship(__instance);
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }

        __result = (int) (__instance.price.Value * adjustedFriendship);
        return false; // don't run original logic
    }

    #endregion harmony patches
}