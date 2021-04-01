using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			AwesomeProfessions.ProspectorHunt.Update(e.Ticks);
		}
	}
}