namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuIsValidCraftPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuIsValidCraftPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ForgeMenuIsValidCraftPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.IsValidCraft));
    }

    #region harmony patches

    /// <summary>Allow forging Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuIsValidCraftPrefix(ref bool __result, Item? left_item, Item? right_item)
    {
        if (left_item?.QualifiedItemId != QIDs.IridiumBand ||
            right_item?.QualifiedItemId != QIDs.GalaxySoul)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
