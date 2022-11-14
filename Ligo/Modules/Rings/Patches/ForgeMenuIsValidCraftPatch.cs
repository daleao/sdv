namespace DaLion.Ligo.Modules.Rings.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftPatch"/> class.</summary>
    internal ForgeMenuIsValidCraftPatch()
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPostfix]
    private static void ForgeMenuIsValidCraftPostfix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item is Ring { ParentSheetIndex: Constants.IridiumBandIndex } &&
            right_item?.ParentSheetIndex == Constants.GalaxySoulIndex)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
