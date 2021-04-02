using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
	{
		/// <inheritdoc/>
		public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (AwesomeProfessions.Config.ModKey.JustPressed()) AwesomeProfessions.EventManager.Subscribe(new ArrowPointerUpdateTickedEvent(), new TrackerRenderedHudEvent());

			if (AwesomeProfessions.Config.ModKey.GetState() == SButtonState.Released)
			{
				AwesomeProfessions.EventManager.Unsubscribe(typeof(TrackerRenderedHudEvent));
				if (!(AwesomeProfessions.EventManager.IsListening(typeof(ProspectorHuntRenderedHudEvent)) || AwesomeProfessions.EventManager.IsListening(typeof(ScavengerHuntRenderedHudEvent))))
					AwesomeProfessions.EventManager.Unsubscribe(typeof(ArrowPointerUpdateTickedEvent));
			}
		}
	}
}