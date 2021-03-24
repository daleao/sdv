using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal class TrackerButtonsChangedEvent : ButtonsChangedEvent
	{
		private EventManager _Manager { get; }

		/// <summary>Construct an instance.</summary>
		internal TrackerButtonsChangedEvent(EventManager manager)
		{
			_Manager = manager;
		}

		/// <summary>Raised after the player released a keyboard, mouse, or controller button. Hook tracking pointer rendering events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
		{
			if (Config.ModKey.JustPressed()) _Manager.Subscribe(new ArrowPointerUpdateTickedEvent(), new TrackerRenderingHudEvent());

			if (Config.ModKey.GetState() == SButtonState.Released)
			{
				_Manager.Unsubscribe(typeof(TrackerRenderingHudEvent));
				if (!(_Manager.IsListening(typeof(ProspectorHuntRenderingHudEvent)) || _Manager.IsListening(typeof(ScavengerHuntRenderingHudEvent))))
					_Manager.Unsubscribe(typeof(ArrowPointerUpdateTickedEvent));
			}
		}
	}
}
