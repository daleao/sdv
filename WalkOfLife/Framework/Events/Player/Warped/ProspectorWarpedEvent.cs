using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ProspectorHunt ??= new();
			if (ModEntry.ProspectorHunt.TreasureTile is not null) ModEntry.ProspectorHunt.End();
			if (Game1.CurrentEvent is null && e.NewLocation is MineShaft)
				ModEntry.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}