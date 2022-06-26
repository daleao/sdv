namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewValley.Menus;

using Common.Data;
using Common.Harmony;

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
    private static void ItemGrabMenuReadyToClosePostfix(ItemGrabMenu __instance)
    {
        if (__instance.context is not FishPond pond) return;

        var items = __instance.ItemsToGrabMenu?.actualInventory;
        if (items is null || !items.Any() || items.All(i => i is null))
        {
            ModDataIO.WriteData(pond, "ItemsHeld", null);
            pond.output.Value = null;
            return;
        }

        var objects = items.Cast<SObject>().ToList();
        var output = objects.OrderByDescending(o => o?.Price).First();
        objects.Remove(output);
        if (objects.Any() && !objects.All(o => o is null))
        {
            var data = objects.Select(o => $"{o.ParentSheetIndex},{o.Stack},{o.Quality}");
            ModDataIO.WriteData(pond, "ItemsHeld", string.Join(';', data));
        }
        else
        {
            ModDataIO.WriteData(pond, "ItemsHeld", null);
        }

        pond.output.Value = output;
    }

    #endregion harmony patches
}