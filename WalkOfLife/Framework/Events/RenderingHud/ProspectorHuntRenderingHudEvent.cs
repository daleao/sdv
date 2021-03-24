using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorHuntRenderingHudEvent : RenderingHudEvent
	{
		private ProspectorHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ProspectorHuntRenderingHudEvent(ProspectorHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. Render prospector hunt target indicator.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (_Hunt.TreasureTile != null)
			{
				var distanceSquared = (Game1.player.getTileLocation() - _Hunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(Config.TreasureTileDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(_Hunt.TreasureTile.Value, Color.Violet);
			}
		}
	}
}
