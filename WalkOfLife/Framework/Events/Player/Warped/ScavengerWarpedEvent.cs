using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ScavengerHunt ??= new();
			if (ModEntry.ScavengerHunt.TreasureTile is not null) ModEntry.ScavengerHunt.End();
			if (Game1.CurrentEvent is null && e.NewLocation.IsOutdoors &&
			    !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName == "Town"))
				ModEntry.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}