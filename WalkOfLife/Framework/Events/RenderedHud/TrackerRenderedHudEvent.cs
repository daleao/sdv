using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerRenderedHudEvent : RenderedHudEvent
	{
		/// <inheritdoc/>
		public override void OnRenderedHud(object sender, RenderedHudEventArgs e)
		{
			// reveal on-sreen trackable objects
			foreach (var pair in Game1.currentLocation.Objects.Pairs.Where(p => Utility.ShouldPlayerTrackObject(p.Value)))
				Utility.DrawArrowPointerOverTarget(pair.Key, Color.Yellow);

			// reveal on-screen ladders and shafts
			if (Utility.LocalPlayerHasProfession("Prospector") && Game1.currentLocation is MineShaft)
				foreach (Vector2 tile in Utility.GetLadderTiles(Game1.currentLocation as MineShaft)) Utility.DrawTrackingArrowPointer(tile, Color.Lime);
		}
	}
}