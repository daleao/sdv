namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemGrabMenuCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<ItemGrabMenu>(typeof(List<Item>), typeof(object));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemGrabMenuCtorPostfix(ItemGrabMenu __instance)
    {
        if (__instance.context is not PipedSlime)
        {
            return;
        }

        __instance.ItemsToGrabMenu.capacity = 12;
        __instance.ItemsToGrabMenu.height -= 2 * Game1.tileSize;
        __instance.ItemsToGrabMenu.rows = 1;
        __instance.ItemsToGrabMenu.showGrayedOutSlots = false;
        __instance.ItemsToGrabMenu.xPositionOnScreen += 4;
        __instance.canExitOnKey = true;
    }

    #endregion harmony patches
}
