using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorWarpedEvent : WarpedEvent
	{
		private ProspectorHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ProspectorWarpedEvent(ProspectorHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised after the current player moves to a new location. Trigger Prospector hunt events + track initial ladder down.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			if (_Hunt.TreasureTile != null) _Hunt.End();

			AwesomeProfessions.initialLadderTiles.Clear();
			if (e.NewLocation is MineShaft)
			{
				foreach (var tile in Utility.GetLadderTiles(e.NewLocation as MineShaft)) AwesomeProfessions.initialLadderTiles.Add(tile);

				if (Game1.CurrentEvent == null) _Hunt.TryStartNewHunt(e.NewLocation);
			}
		}
	}
}
