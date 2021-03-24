using StardewModdingAPI.Events;
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	internal class SpelunkerWarpedEvent : WarpedEvent
	{
		/// <summary>Construct an instance.</summary>
		internal SpelunkerWarpedEvent() { }

		/// <summary>Raised after the current player moves to a new location. Record Spelunker lowest level reached.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && e.NewLocation is MineShaft)
			{
				uint currentMineLevel = (uint)(e.NewLocation as MineShaft).mineLevel;
				if (currentMineLevel > Data.LowestMineLevelReached) Data.LowestMineLevelReached = currentMineLevel;
			}
		}
	}
}
