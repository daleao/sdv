using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (e.Ticks % 4 == 0) Utility.ArrowPointer.Bob();
		}
	}
}