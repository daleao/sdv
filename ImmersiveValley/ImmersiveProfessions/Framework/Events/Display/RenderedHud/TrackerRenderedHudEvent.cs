namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Extensions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal TrackerRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        // reveal on-screen trackable objects
        foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => p.Value.ShouldBeTracked()))
            ModEntry.PlayerState.Pointer.DrawOverTile(pair.Key, Color.Yellow);

        if (!Game1.player.HasProfession(Profession.Prospector) || Game1.currentLocation is not MineShaft shaft) return;

        // reveal on-screen ladders and shafts
        foreach (var tile in shaft.GetLadderTiles()) ModEntry.PlayerState.Pointer.DrawOverTile(tile, Color.Lime);
    }
}