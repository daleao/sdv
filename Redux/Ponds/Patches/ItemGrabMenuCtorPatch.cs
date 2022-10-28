namespace DaLion.Redux.Ponds.Patches;

#region using directives

using System.Collections.Generic;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuCtorPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuCtorPatch"/> class.</summary>
    internal ItemGrabMenuCtorPatch()
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
