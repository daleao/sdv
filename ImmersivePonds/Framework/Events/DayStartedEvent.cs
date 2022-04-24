namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Extensions;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.DayStarted"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class DayStartedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted += OnDayStarted;
        Log.D("[Ponds] Hooked DayStarted event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted -= OnDayStarted;
        Log.D("[Ponds] Unhooked DayStarted event.");
    }

    /// <summary>Raised before the game writes data to save file.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
            pond.WriteData("CheckedToday", false.ToString());
    }
}