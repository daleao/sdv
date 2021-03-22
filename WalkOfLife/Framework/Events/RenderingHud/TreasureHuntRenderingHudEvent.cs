using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class TreasureHuntRenderingHudEvent : RenderingHudEvent
	{
		/// <summary>Construct an instance.</summary>
		internal TreasureHuntRenderingHudEvent() { }

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. Render treasure hunt target indicator.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (TreasureHunt.TreasureTile != null)
			{
				if (Game1.currentLocation.IsOutdoors)
					Utility.DrawTrackingArrowPointer(TreasureHunt.TreasureTile.Value, Color.Violet);

				var distanceSquared = (Game1.player.getTileLocation() - TreasureHunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(_config.TreasureTileDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(TreasureHunt.TreasureTile.Value, Color.Violet);
			}
		}
	}
}
