namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalGetSellPricePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalGetSellPricePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmAnimalGetSellPricePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.getSellPrice));
    }

    #region harmony patches

    /// <summary>Patch to adjust Breeder animal sell price.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmAnimalGetSellPricePrefix(FarmAnimal __instance, ref int __result)
    {
        if (!__instance.GetOwner().HasProfessionOrLax(Profession.Breeder))
        {
            return true; // run original logic
        }

        try
        {
            __result = (int)((__instance.GetAnimalData()?.SellPrice ?? 1) * __instance.GetBreederAdjustedFriendship());
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
