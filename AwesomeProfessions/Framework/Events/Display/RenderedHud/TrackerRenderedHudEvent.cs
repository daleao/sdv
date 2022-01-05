﻿using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events.Display.RenderedHud;

internal class TrackerRenderedHudEvent : RenderedHudEvent
{
    /// <inheritdoc />
    public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
    {
        // reveal on-sreen trackable objects
        foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => p.Value.ShouldBeTracked()))
            HUD.DrawArrowPointerOverTarget(pair.Key, Color.Yellow);

        if (!Game1.player.HasProfession("Prospector") || Game1.currentLocation is not MineShaft shaft) return;

        // reveal on-screen ladders and shafts
        foreach (var tile in shaft.GetLadderTiles()) HUD.DrawTrackingArrowPointer(tile, Color.Lime);
    }
}