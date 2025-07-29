namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectMinutesElapsedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectMinutesElapsedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectMinutesElapsedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.minutesElapsed));
    }

    #region harmony patches

    /// <summary>Patch to make Hopper actually useful. Attempt load to chest underneath.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectMinutesElapsedPostfix(SObject __instance)
    {
        var location = __instance.Location;
        if (location is null || !__instance.TryGetHopper(out var hopper))
        {
            return;
        }

        var tileBelowHopper = new Vector2(hopper.TileLocation.X, hopper.TileLocation.Y + 1f);
        if (!location.Objects.TryGetValue(tileBelowHopper, out var @object) || @object is not Chest chest ||
            chest.SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
        {
            return;
        }

        while (hopper.Items.Count > 0 && hopper.Items[0] is { } item && chest.addItem(item) is null)
        {
            hopper.Items.RemoveAt(0);
        }
    }

    #endregion harmony patches
}
