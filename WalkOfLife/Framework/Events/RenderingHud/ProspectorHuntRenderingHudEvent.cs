using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorHuntRenderingHudEvent : RenderingHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			if (AwesomeProfessions.ProspectorHunt.TreasureTile != null)
			{
				var distanceSquared = (Game1.player.getTileLocation() - AwesomeProfessions.ProspectorHunt.TreasureTile.Value).LengthSquared();
				if (distanceSquared <= Math.Pow(AwesomeProfessions.Config.TreasureTileDetectionDistance, 2))
					Utility.DrawArrowPointerOverTarget(AwesomeProfessions.ProspectorHunt.TreasureTile.Value, Color.Violet);
			}
		}
	}
}