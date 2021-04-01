using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHuntRenderingHudEvent : RenderingHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (AwesomeProfessions.ScavengerHunt.TreasureTile != null)
			{
				Utility.DrawTrackingArrowPointer(AwesomeProfessions.ScavengerHunt.TreasureTile.Value, Color.Violet);

				var distanceSquared = (Game1.player.getTileLocation() - AwesomeProfessions.ScavengerHunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(AwesomeProfessions.Config.TreasureTileDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(AwesomeProfessions.ScavengerHunt.TreasureTile.Value, Color.Violet);
			}
		}
	}
}