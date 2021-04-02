using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class RenderedHudEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Display.RenderedHud += OnRenderedHud;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Display.RenderedHud -= OnRenderedHud;
		}

		/// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderedHud(object sender, RenderedHudEventArgs e);
	}
}