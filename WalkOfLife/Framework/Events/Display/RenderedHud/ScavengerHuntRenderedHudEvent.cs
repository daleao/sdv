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
			if (!ModState.ScavengerHunt.IsActive) return;

			// track and reveal treasure hunt target
			HUD.DrawTrackingArrowPointer(ModState.ScavengerHunt.TreasureTile.Value, Color.Violet);
			var distanceSquared = (Game1.player.getTileLocation() - ModState.ScavengerHunt.TreasureTile.Value)
				.LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				HUD.DrawArrowPointerOverTarget(ModState.ScavengerHunt.TreasureTile.Value, Color.Violet);
		}
	}
}