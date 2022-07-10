namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common.Data;
using Common.Extensions.Collections;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuReadyToClosePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ItemGrabMenuReadyToClosePatch()
    {
        Target = RequireMethod<ItemGrabMenu>(nameof(ItemGrabMenu.readyToClose));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    private static void ItemGrabMenuReadyToClosePostfix(ItemGrabMenu __instance, ref bool __result)
    {
        if (__instance.context is not FishPond pond) return;

        var inventory = __instance.ItemsToGrabMenu?.actualInventory.WhereNotNull().ToList();
        if (inventory?.Count is not > 0)
        {
            ModDataIO.WriteTo(pond, "ItemsHeld", null);
            pond.output.Value = null;
            return;
        }

        var output = inventory.OrderByDescending(i => i is ColoredObject
            ? new SObject(i.ParentSheetIndex, 1).salePrice()
            : i.salePrice())
        .First() as SObject;
        inventory.Remove(output!);
        if (inventory.Count > 0)
        {
            var serialized = inventory.Select(i => $"{i.ParentSheetIndex},{i.Stack},{((SObject)i).Quality}");
            ModDataIO.WriteTo(pond, "ItemsHeld", string.Join(';', serialized));
        }
        else
        {
            ModDataIO.WriteTo(pond, "ItemsHeld", null);
        }

        pond.output.Value = output;
        __result = true; // ready to close
    }

    #endregion harmony patches
}