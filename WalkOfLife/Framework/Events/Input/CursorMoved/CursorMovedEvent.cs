using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal abstract class CursorMovedEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Input.CursorMoved += OnCursorMoved;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Input.CursorMoved -= OnCursorMoved;
		}

		/// <summary>Raised after the player moves the in-game cursor.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnCursorMoved(object sender, CursorMovedEventArgs e);
	}
}