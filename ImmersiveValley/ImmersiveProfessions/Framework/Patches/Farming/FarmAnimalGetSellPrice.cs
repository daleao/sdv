namespace DaLion.Stardew.Professions.Framework.Patches.Farming;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalGetSellPricePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmAnimalGetSellPricePatch()
    {
        Target = RequireMethod<FarmAnimal>(nameof(FarmAnimal.getSellPrice));
    }

    #region harmony patches

    /// <summary>Patch to adjust Breeder animal sell price.</summary>
    [HarmonyPrefix]
    private static bool FarmAnimalGetSellPricePrefix(FarmAnimal __instance, ref int __result)
    {
        double adjustedFriendship;
        try
        {
            if (!__instance.GetOwner().HasProfession(Profession.Breeder)) return true; // run original logic

            adjustedFriendship = __instance.GetProducerAdjustedFriendship();
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }

        __result = (int)(__instance.price.Value * adjustedFriendship);
        return false; // don't run original logic
    }

    #endregion harmony patches
}