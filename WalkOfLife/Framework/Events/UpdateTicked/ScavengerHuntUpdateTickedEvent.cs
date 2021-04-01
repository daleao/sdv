using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			AwesomeProfessions.ScavengerHunt.Update(e.Ticks);
		}
	}
}