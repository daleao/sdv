﻿namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class IslandNorthGetIslandMerchantTradeStockPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="IslandNorthGetIslandMerchantTradeStockPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal IslandNorthGetIslandMerchantTradeStockPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<IslandNorth>(nameof(IslandNorth.getIslandMerchantTradeStock));
    }

    #region harmony patches

    /// <summary>Remove Dragon Tooth from Island Trader.</summary>
    [HarmonyPostfix]
    private static void IslandNorthGetIslandMerchantTradeStockPostfix(Dictionary<ISalable, int[]> __result)
    {
        if (!CombatModule.Config.Quests.DwarvenLegacy)
        {
            return;
        }

        var bananaSapling = __result.Keys.FirstOrDefault(i => i is SObject { ParentSheetIndex: ObjectIds.BananaSapling });
        if (bananaSapling is null)
        {
            return;
        }

        __result[bananaSapling][2] = 719;
        __result[bananaSapling][3] = 75;
    }

    #endregion harmony patches
}
