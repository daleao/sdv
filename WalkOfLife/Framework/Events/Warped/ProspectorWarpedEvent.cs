using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			if (AwesomeProfessions.ProspectorHunt.TreasureTile != null) AwesomeProfessions.ProspectorHunt.End();

			AwesomeProfessions.initialLadderTiles.Clear();
			if (e.NewLocation is MineShaft)
			{
				foreach (var tile in Utility.GetLadderTiles(e.NewLocation as MineShaft)) AwesomeProfessions.initialLadderTiles.Add(tile);

				if (Game1.CurrentEvent == null) AwesomeProfessions.ProspectorHunt.TryStartNewHunt(e.NewLocation);
			}
		}
	}
}