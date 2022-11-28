namespace DaLion.Ligo.Modules.Ponds.Patchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuCtorPatcher"/> class.</summary>
    internal ItemGrabMenuCtorPatcher()
    {
        this.Target = this.RequireConstructor<ItemGrabMenu>(typeof(List<Item>), typeof(object));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    private static void ItemGrabMenuCtorPostfix(ItemGrabMenu __instance)
    {
        if (__instance.context is FishPond)
        {
            __instance.canExitOnKey = true;
        }
    }

    #endregion harmony patches
}
