namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
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

    /// <inheritdoc />
    protected override bool ApplyImpl(Harmony harmony)
    {
        return ModHelper.ModRegistry.IsLoaded("Pathoschild.Automate") || base.ApplyImpl(harmony);
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
        if (!location.Objects.TryGetValue(tileBelowHopper, out var objectBelow) || objectBelow.GetMachineData() is null)
        {
            return;
        }

        var owner = hopper.GetOwner();
        var tileAboveHopper = new Vector2(hopper.TileLocation.X, hopper.TileLocation.Y - 1);
        if (!location.Objects.TryGetValue(tileAboveHopper, out var objectAbove))
        {
            return;
        }

        var chest = objectAbove as Chest ?? objectAbove.heldObject.Value as Chest;
        if (chest is null)
        {
            return;
        }

        AttemptAutoLoad(chest, objectBelow, owner);
    }

    #endregion harmony patches

    public static Task<bool> AttemptAutoLoad(Chest source, SObject destination, Farmer who)
    {
        var taskSource = new TaskCompletionSource<bool>();
        source.GetMutex().RequestLock(() =>
        {
            try
            {
                source.GetMutex().ReleaseLock();
                var result = destination.AttemptAutoLoad(source.Items, who);
                taskSource.SetResult(result);
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }
        });

        return taskSource.Task;
    }
}
