using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderingHudEvent : RenderingHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderingHud(object sender, RenderingHudEventArgs e)
		{
			foreach (var kvp in Game1.currentLocation.Objects.Pairs)
				if (Utility.ShouldPlayerTrackObject(kvp.Value)) Utility.DrawArrowPointerOverTarget(kvp.Key, Color.Yellow);
		}
	}
}