namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarven;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class IslandNorthGetIslandMerchantTradeStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IslandNorthGetIslandMerchantTradeStockPatcher"/> class.</summary>
    internal IslandNorthGetIslandMerchantTradeStockPatcher()
    {
        this.Target = this.RequireMethod<IslandNorth>(nameof(IslandNorth.getIslandMerchantTradeStock));
    }

    #region harmony patches

    /// <summary>Remove Dragon Tooth from Island Trader.</summary>
    [HarmonyPostfix]
    private static void IslandNorthGetIslandMerchantTradeStockPostfix(Dictionary<ISalable, int[]> __result)
    {
        if (!WeaponsModule.Config.DwarvenLegacy)
        {
            return;
        }

        var bananaSapling = __result.Keys.FirstOrDefault(i => i is SObject { ParentSheetIndex: ItemIDs.BananaSapling });
        if (bananaSapling is null)
        {
            return;
        }

        __result[bananaSapling][2] = 719;
        __result[bananaSapling][3] = 75;
    }

    #endregion harmony patches
}
