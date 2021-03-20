using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	public class ProspectorWarpedEvent : BaseWarpedEvent
	{
		/// <summary>Raised after the current player moves to a new location. Trigger Prospector hunt events + track initial ladder down.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer || Game1.CurrentEvent != null) return;

			AwesomeProfessions.InitialLadderTiles.Clear();
			if (e.NewLocation is MineShaft)
			{
				foreach (var tile in Utility.GetLadderTiles(e.NewLocation as MineShaft)) AwesomeProfessions.InitialLadderTiles.Add(tile);

				AwesomeProfessions.ProspectorHunt.TryStartNewHunt(e.NewLocation);
			}
		}
	}
}
