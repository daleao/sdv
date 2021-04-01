using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			if (AwesomeProfessions.ScavengerHunt.TreasureTile != null) AwesomeProfessions.ScavengerHunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors && !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName.Equals("Town")))
				AwesomeProfessions.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}