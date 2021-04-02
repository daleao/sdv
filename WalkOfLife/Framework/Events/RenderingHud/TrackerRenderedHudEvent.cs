using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			foreach (var kvp in Game1.currentLocation.Objects.Pairs)
				if (Utility.ShouldPlayerTrackObject(kvp.Value)) Utility.DrawArrowPointerOverTarget(kvp.Key, Color.Yellow);
		}
	}
}