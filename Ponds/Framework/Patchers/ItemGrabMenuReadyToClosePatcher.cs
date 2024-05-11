namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemGrabMenuReadyToClosePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemGrabMenuReadyToClosePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ItemGrabMenuReadyToClosePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MenuWithInventory>(nameof(MenuWithInventory.readyToClose));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    private static void ItemGrabMenuReadyToClosePostfix(ItemGrabMenu __instance, ref bool __result)
    {
        if (__instance.context is not FishPond pond)
        {
            return;
        }

        var inventory = __instance.ItemsToGrabMenu?.actualInventory.WhereNotNull().ToList();
        if (inventory?.Count is not > 0)
        {
            Data.Write(pond, DataKeys.ItemsHeld, null);
            pond.output.Value = null;
            return;
        }

        var output = inventory
            .OrderByDescending(i => i is ColoredObject
                ? ItemRegistry.Create<SObject>(i.QualifiedItemId, 1).salePrice()
                : i.salePrice())
            .First() as SObject;
        inventory.Remove(output!);
        if (inventory.Count > 0)
        {
            var serialized = inventory
                .Select(i => $"{i.QualifiedItemId},{i.Stack},{((SObject)i).Quality}");
            Data.Write(pond, DataKeys.ItemsHeld, string.Join(';', serialized));
        }
        else
        {
            Data.Write(pond, DataKeys.ItemsHeld, null);
        }

        pond.output.Value = output;
        __result = true; // ready to close
    }

    #endregion harmony patches
}
