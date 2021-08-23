using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class RenderedHudEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.Events.Display.RenderedHud += OnRenderedHud;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.Events.Display.RenderedHud -= OnRenderedHud;
		}

		/// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderedHud(object sender, RenderedHudEventArgs e);
	}
}