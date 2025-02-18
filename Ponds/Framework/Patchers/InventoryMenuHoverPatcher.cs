﻿namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class InventoryMenuHoverPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="InventoryMenuHoverPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal InventoryMenuHoverPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<InventoryMenu>(nameof(InventoryMenu.hover));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemGrabMenuCtorPostfix(InventoryMenu __instance, Item __result, Item heldItem)
    {
        if (__result is not null)
        {
            return;
        }

        return;
    }

    #endregion harmony patches
}
