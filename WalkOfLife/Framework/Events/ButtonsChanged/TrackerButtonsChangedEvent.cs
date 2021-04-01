using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
	{
		/// <inheritdoc/>
		public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (AwesomeProfessions.Config.ModKey.JustPressed()) AwesomeProfessions.EventManager.Subscribe(new ArrowPointerUpdateTickedEvent(), new TrackerRenderingHudEvent());

			if (AwesomeProfessions.Config.ModKey.GetState() == SButtonState.Released)
			{
				AwesomeProfessions.EventManager.Unsubscribe(typeof(TrackerRenderingHudEvent));
				if (!(AwesomeProfessions.EventManager.IsListening(typeof(ProspectorHuntRenderingHudEvent)) || AwesomeProfessions.EventManager.IsListening(typeof(ScavengerHuntRenderingHudEvent))))
					AwesomeProfessions.EventManager.Unsubscribe(typeof(ArrowPointerUpdateTickedEvent));
			}
		}
	}
}