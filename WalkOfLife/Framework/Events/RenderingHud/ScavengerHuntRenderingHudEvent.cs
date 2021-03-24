using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHuntRenderingHudEvent : RenderingHudEvent
	{
		private ScavengerHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ScavengerHuntRenderingHudEvent(ScavengerHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. Render scavenger hunt target indicator.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (_Hunt.TreasureTile != null)
			{
				Utility.DrawTrackingArrowPointer(_Hunt.TreasureTile.Value, Color.Violet);

				var distanceSquared = (Game1.player.getTileLocation() - _Hunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(Config.TreasureTileDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(_Hunt.TreasureTile.Value, Color.Violet);
			}
		}
	}
}
