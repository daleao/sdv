namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmAnimalGetSellPricePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmAnimalGetSellPricePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmAnimalGetSellPricePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.getSellPrice));
    }

    #region harmony patches

    /// <summary>Patch to adjust Breeder animal sell price.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmAnimalGetSellPricePostfix(FarmAnimal __instance, ref int __result)
    {
        if (!__instance.DoesOwnerHaveProfessionOrLax(Profession.Breeder))
        {
            return; // do not change sell price
        }

        try
        {
            __result = (int)(__result * __instance.GetBreederAdjustedPrice()); 
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return; // default to original logic
        }
    }

    #endregion harmony patches
}
