using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (AwesomeProfessions.ScavengerHunt.TreasureTile == null) return;

			// track and reveal treasure hunt target
			Utility.DrawTrackingArrowPointer(AwesomeProfessions.ScavengerHunt.TreasureTile.Value, Color.Violet);
			var distanceSquared = (Game1.player.getTileLocation() - AwesomeProfessions.ScavengerHunt.TreasureTile.Value).LengthSquared();
			if (distanceSquared <= Math.Pow(AwesomeProfessions.Config.TreasureTileDetectionDistance, 2))
				Utility.DrawArrowPointerOverTarget(AwesomeProfessions.ScavengerHunt.TreasureTile.Value, Color.Violet);
		}
	}
}