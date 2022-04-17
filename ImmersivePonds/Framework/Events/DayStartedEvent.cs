namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using System.Collections.Generic;
using System.Linq;
using StardewValley.Buildings;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Extensions;

#endregion using directives

internal class DayStartedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted += OnDayStarted;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.DayStarted -= OnDayStarted;
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