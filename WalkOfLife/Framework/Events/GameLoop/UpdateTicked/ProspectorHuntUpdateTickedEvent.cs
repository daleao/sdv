using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			ModState.ProspectorHunt.Update(e.Ticks);
		}
	}
}