namespace DaLion.Stardew.Ponds.Framework.Events;

#region using directives

using Common.Data;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PondDayStartedEvent : DayStartedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PondDayStartedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        if (!Context.IsMainPlayer) return;

        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
            ModDataIO.WriteTo(pond, "CheckedToday", false.ToString());
    }
}