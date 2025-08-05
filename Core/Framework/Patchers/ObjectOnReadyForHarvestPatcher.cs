namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectOnReadyForHarvestPatcher : HarmonyPatcher
{
    private static Lazy<Func<SObject, Farmer, bool, bool>> CheckForActionOnMachineDelegate = new(() => Reflector
        .GetUnboundMethodDelegate<Func<SObject, Farmer, bool, bool>>(typeof(SObject), "CheckForActionOnMachine"));

    /// <summary>Initializes a new instance of the <see cref="ObjectOnReadyForHarvestPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectOnReadyForHarvestPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.onReadyForHarvest));
    }

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        return ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate") || base.ApplyImpl(harmony);
    }

    #region harmony patches

    /// <summary>Patch to make Hopper actually useful.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectOnReadyForHarvestPostfix(SObject __instance)
    {
        var location = __instance.Location;
        if (location is null)
        {
            return;
        }

        var tileBelow = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y + 1f);
        if (location.TryGetHopperAt(tileBelow, out var hopperBelow))
        {
            CheckForActionOnMachineDelegate.Value(__instance, hopperBelow.GetOwner(), false);
            return;
        }

        var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
        if (location.TryGetHopperAt(tileAbove, out var hopperAbove))
        {
            CheckForActionOnMachineDelegate.Value(__instance, hopperAbove.GetOwner(), false);
        }
    }

    #endregion harmony patches
}
