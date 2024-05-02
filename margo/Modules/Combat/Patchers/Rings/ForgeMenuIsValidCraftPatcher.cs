namespace DaLion.Overhaul.Modules.Combat.Patchers.Rings;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal ForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuIsValidCraftPrefix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item is not Ring { ParentSheetIndex: ObjectIds.IridiumBand } ||
            right_item?.ParentSheetIndex != ObjectIds.GalaxySoul)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
