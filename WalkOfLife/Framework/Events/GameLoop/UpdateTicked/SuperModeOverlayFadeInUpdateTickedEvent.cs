using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeOverlayFadeInUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			ModEntry.SuperModeOverlayAlpha += 0.01f;
			if (ModEntry.SuperModeOverlayAlpha >= 0.3f) ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}