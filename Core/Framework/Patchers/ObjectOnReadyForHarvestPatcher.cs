namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectOnReadyForHarvestPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectOnReadyForHarvestPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectOnReadyForHarvestPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.onReadyForHarvest));
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
        if (location.Objects.TryGetValue(tileBelow, out var toObj) &&
            toObj is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopperBelow)
        {
            Reflector
                .GetUnboundMethodDelegate<Func<SObject, Farmer, bool, bool>>(__instance, "CheckForActionOnMachine")
                .Invoke(__instance, hopperBelow.GetOwner(), false);
            return;
        }

        var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
        if (location.Objects.TryGetValue(tileAbove, out toObj) &&
            toObj is Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopperAbove)
        {
            Reflector
                .GetUnboundMethodDelegate<Func<SObject, Farmer, bool, bool>>(__instance, "CheckForActionOnMachine")
                .Invoke(__instance, hopperAbove.GetOwner(), false);
        }
    }

    #endregion harmony patches
}
