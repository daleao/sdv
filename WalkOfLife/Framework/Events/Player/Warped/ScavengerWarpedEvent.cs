using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ScavengerHunt ??= new ScavengerHunt();
			if (ModEntry.ScavengerHunt.TreasureTile != null) ModEntry.ScavengerHunt.End();
			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors &&
				!(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName == "Town"))
				ModEntry.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}