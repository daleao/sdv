using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class BruteWarpedEvent : WarpedEvent
	{
		/// <inheritdoc/>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (e.IsLocalPlayer && AwesomeProfessions.bruteKillStreak > 0 && e.NewLocation.GetType() != e.OldLocation.GetType())
				AwesomeProfessions.bruteKillStreak = 0;
		}
	}
}