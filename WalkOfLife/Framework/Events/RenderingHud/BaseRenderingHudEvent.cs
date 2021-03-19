using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public abstract class BaseRenderingHudEvent : IEvent
	{
		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents listener)
		{
			listener.Display.RenderingHud += OnRenderingHud;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents listener)
		{
			listener.Display.RenderingHud -= OnRenderingHud;
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderingHud(object sender, RenderingHudEventArgs e);
	}
}
