using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	internal abstract class InventoryChangedEvent : IEvent
	{
		/// <inheritdoc/>
		public void Hook()
		{
			AwesomeProfessions.Events.Player.InventoryChanged += OnInventoryChanged;
		}

		/// <inheritdoc/>
		public void Unhook()
		{
			AwesomeProfessions.Events.Player.InventoryChanged -= OnInventoryChanged;
		}

		/// <summary>Raised after items are added or removed from the player inventory.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnInventoryChanged(object sender, InventoryChangedEventArgs e);
	}
}