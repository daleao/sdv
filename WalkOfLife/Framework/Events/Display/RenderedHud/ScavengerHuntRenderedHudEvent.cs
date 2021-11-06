using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc />
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModEntry.ScavengerHunt.TreasureTile is null) return;

			// track and reveal treasure hunt target
			HUD.DrawTrackingArrowPointer(ModEntry.ScavengerHunt.TreasureTile.Value, Color.Violet);
			var distanceSquared = (Game1.player.getTileLocation() - ModEntry.ScavengerHunt.TreasureTile.Value)
				.LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				HUD.DrawArrowPointerOverTarget(ModEntry.ScavengerHunt.TreasureTile.Value, Color.Violet);
		}
	}
}