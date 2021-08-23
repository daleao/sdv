using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModEntry.ProspectorHunt.TreasureTile == null) return;

			// reveal treasure hunt target
			var distanceSquared = (Game1.player.getTileLocation() - ModEntry.ProspectorHunt.TreasureTile.Value).LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				Util.HUD.DrawArrowPointerOverTarget(ModEntry.ProspectorHunt.TreasureTile.Value, Color.Violet);
		}
	}
}