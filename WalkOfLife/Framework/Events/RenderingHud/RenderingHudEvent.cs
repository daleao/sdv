using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class RenderingHudEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal RenderingHudEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.Display.RenderingHud += OnRenderingHud;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.Display.RenderingHud -= OnRenderingHud;
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderingHud(object sender, RenderingHudEventArgs e);
	}
}
