using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			// reveal on-sreen trackable objects
			foreach (var kvp in Game1.currentLocation.Objects.Pairs)
				if (Utility.ShouldPlayerTrackObject(kvp.Value)) Utility.DrawArrowPointerOverTarget(kvp.Key, Color.Yellow);

			// reveal on-screen ladders and shafts
			if (Utility.LocalPlayerHasProfession("Prospector") && Game1.currentLocation is MineShaft)
				foreach (var tile in Utility.GetLadderTiles(Game1.currentLocation as MineShaft)) Utility.DrawTrackingArrowPointer(tile, Color.Lime);
		}
	}
}