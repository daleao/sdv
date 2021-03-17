using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using System.Collections.Generic;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		public static List<Vector2> InitialLadderTiles { get; } = new();

		/// <summary>Raised after the current player moves to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			InitialLadderTiles.Clear();
			if (e.NewLocation is MineShaft)
			{
				// find ladder positions for Prospector
				if (Utility.SpecificPlayerHasProfession("prospector", e.Player))
					foreach (var tile in Utility.GetLadderTiles(e.NewLocation as MineShaft)) InitialLadderTiles.Add(tile);

				// record lowest level reached for local player
				uint currentMineLevel = (uint)(e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached) Data.LowestMineLevelReached = currentMineLevel;
			}

			// reset Brute buff
			if (BruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType()) BruteKillStreak = 0;

			if (Utility.SpecificPlayerHasProfession("scavenger", e.Player) && e.NewLocation.IsOutdoors)
			{ }
			else if (Utility.SpecificPlayerHasProfession("prospector", e.Player) && e.NewLocation is MineShaft)
			{ }
		}
	}
}
