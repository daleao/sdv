namespace DaLion.Redux.Framework.Rings.Patches;

#region using directives

using HarmonyLib;
using SpaceCore.Interface;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class NewForgeMenuIsValidCraftPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="NewForgeMenuIsValidCraftPatch"/> class.</summary>
    internal NewForgeMenuIsValidCraftPatch()
    {
        this.Target = this.RequireMethod<NewForgeMenu>(nameof(NewForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPostfix]
    private static void NewForgeMenuIsValidCraftPostfix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item is Ring { ParentSheetIndex: Constants.IridiumBandIndex } &&
            right_item?.ParentSheetIndex == Constants.GalaxySoulIndex)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
