namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPlacementActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPlacementActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPlacementActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.placementAction));
    }

    #region harmony patches

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        return ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate") || base.ApplyImpl(harmony);
    }

    /// <summary>Patch to make Hopper actually useful.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectPlacementActionPostfix(SObject __instance)
    {
        var location = __instance.Location;
        if (location is null)
        {
            return;
        }

        if (__instance.QualifiedItemId == QIDs.Hopper)
        {
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
        else
        {
            var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
            if (location.Objects.TryGetValue(tileAbove, out var fromObj) && fromObj is Chest
                { QualifiedItemId: QIDs.Hopper, specialChestType.Value: Chest.SpecialChestTypes.AutoLoader } hopper)
            {
                hopper.CheckAutoLoad(hopper.GetOwner());
            }
        }
    }

    #endregion harmony patches
}
