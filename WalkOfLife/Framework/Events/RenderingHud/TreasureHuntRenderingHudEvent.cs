using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	public class TreasureHuntRenderingHudEvent : BaseRenderingHudEvent
	{
		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. Render treasure hunt target indicator.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (TreasureHunt.TreasureTile != null)
			{
				var distanceSquared = (Game1.player.getTileLocation() - TreasureHunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(AwesomeProfessions.Config.TreasureDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(TreasureHunt.TreasureTile.Value, Color.Violet, e.SpriteBatch);
			}
		}
	}
}
