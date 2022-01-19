namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

#endregion using directives

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

            adjustedFriendship = __instance.GetProducerAdjustedFriendship();
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        __result = (int) (__instance.price.Value * adjustedFriendship);
        return false; // don't run original logic
    }

    #endregion harmony patches
}