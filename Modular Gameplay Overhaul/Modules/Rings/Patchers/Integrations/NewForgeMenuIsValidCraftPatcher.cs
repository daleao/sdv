namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class NewForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftPatcher"/> class.</summary>
    internal NewForgeMenuIsValidCraftPatcher()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftPostfix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item is Ring { ParentSheetIndex: ItemIDs.IridiumBand } &&
            right_item?.ParentSheetIndex == ItemIDs.GalaxySoul)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
