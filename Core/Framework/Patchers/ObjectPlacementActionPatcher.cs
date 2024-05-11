namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectPlacementActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    /// <summary>Patch to make Hopper actually useful.</summary>
    [HarmonyPostfix]
    private static void ObjectPlacementActionPostfix(SObject __instance)
    {
        if (__instance.QualifiedItemId != QualifiedBigCraftableIds.Hopper)
        {
            return;
        }

        var location = __instance.Location;
        if (location is null)
        {
            return;
        }

        var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
        if (location.Objects.TryGetValue(tileAbove, out var fromObj) && fromObj.readyForHarvest.Value)
        {
            fromObj.checkForAction(__instance.GetOwner());
        }

        var tileBelow = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y + 1f);
        if (location.Objects.TryGetValue(tileBelow, out fromObj) && fromObj.readyForHarvest.Value)
        {
            fromObj.checkForAction(__instance.GetOwner());
        }
    }

    #endregion harmony patches
}
