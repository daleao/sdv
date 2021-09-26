using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class RenderedWorldEvent : BaseEvent
	{
		/// <inheritdoc/>
		public override void Hook()
		{
			ModEntry.Events.Display.RenderedWorld += OnRenderedWorld;
		}

		/// <inheritdoc/>
		public override void Unhook()
		{
			ModEntry.Events.Display.RenderedWorld -= OnRenderedWorld;
		}

		/// <summary>Raised after the game world is drawn to the sprite patch, before it's rendered to the screen.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnRenderedWorld(object sender, RenderedWorldEventArgs e);
	}
}