namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ImplicitIgnore]
internal sealed class MenuWithInventoryReadyToClosePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MenuWithInventoryReadyToClosePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MenuWithInventoryReadyToClosePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MenuWithInventory>(nameof(MenuWithInventory.readyToClose));
    }

    #region harmony patches

    /// <summary>Update ItemsHeld data on grab menu close.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MenuWithInventoryToClosePostfix(MenuWithInventory __instance, ref bool __result)
    {
        if (__instance is not ItemGrabMenu { context: Building building } grabMenu ||
            building.GetData()?.DefaultAction != "BuildingSilo")
        {
            return;
        }

        var inventory = grabMenu.ItemsToGrabMenu?.actualInventory.WhereNotNull().ToList();
        if (inventory?.Count is not > 0)
        {
            Data.Write(building, DataKeys.ItemsHeld, null);
            return;
        }

        var serialized = inventory
            .Select(i => $"{i.QualifiedItemId},{i.Stack},{((SObject)i).Quality}");
        Data.Write(building, DataKeys.ItemsHeld, string.Join(';', serialized));
        __result = true; // ready to close
    }

    #endregion harmony patches
}
