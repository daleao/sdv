namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarven;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCouldInventoryAcceptThisItemPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCouldInventoryAcceptThisItemPatcher"/> class.</summary>
    internal FarmerCouldInventoryAcceptThisItemPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.couldInventoryAcceptThisItem));
    }

    #region harmony patches

    /// <summary>Allow picking up blueprints.</summary>
    [HarmonyPrefix]
    private static bool FarmerCouldInventoryAcceptThisItemPrefix(ref bool __result, Item item)
    {
        if (!Globals.DwarvishBlueprintIndex.HasValue || Globals.DwarvishBlueprintIndex.Value != item.ParentSheetIndex)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
