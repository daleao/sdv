namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ForgeMenuGetForgeCostPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ForgeMenuGetForgeCostPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ForgeMenuGetForgeCostPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<ForgeMenu>(nameof(ForgeMenu.GetForgeCost));
    }

    #region harmony patches

    /// <summary>Modify forge cost for Infinity Band.</summary>
    [HarmonyPrefix]
    private static bool ForgeMenuGetForgeCostPrefix(ref int __result, Item left_item, Item right_item)
    {
        if (left_item is not Ring left)
        {
            return true; // run original logic
        }

        if (left.QualifiedItemId == InfinityBandId && right_item is Ring right &&
            Gemstone.TryFromRing(right.QualifiedItemId, out _))
        {
            __result = 10;
            return false; // don't run original logic
        }

        if (left.QualifiedItemId == InfinityBandId && right_item.QualifiedItemId == QualifiedObjectIds.GalaxySoul)
        {
            __result = 20;
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
