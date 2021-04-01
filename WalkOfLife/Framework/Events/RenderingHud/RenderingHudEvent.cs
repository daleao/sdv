using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class RenderingHudEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Display.RenderingHud += OnRenderingHud;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Display.RenderingHud -= OnRenderingHud;
		}

		/// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderingHud(object sender, RenderingHudEventArgs e);
	}
}