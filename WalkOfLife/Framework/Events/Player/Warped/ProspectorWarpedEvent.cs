using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ProspectorHunt ??= new();
			if (ModEntry.ProspectorHunt.TreasureTile != null) ModEntry.ProspectorHunt.End();
			if (Game1.CurrentEvent == null && e.NewLocation is MineShaft) ModEntry.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}