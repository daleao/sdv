using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ProspectorHuntRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc />
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			if (ModEntry.ProspectorHunt.TreasureTile is null) return;

			// reveal treasure hunt target
			var distanceSquared = (Game1.player.getTileLocation() - ModEntry.ProspectorHunt.TreasureTile.Value)
				.LengthSquared();
			if (distanceSquared <= Math.Pow(ModEntry.Config.TreasureDetectionDistance, 2))
				HUD.DrawArrowPointerOverTarget(ModEntry.ProspectorHunt.TreasureTile.Value, Color.Violet);
		}
	}
}