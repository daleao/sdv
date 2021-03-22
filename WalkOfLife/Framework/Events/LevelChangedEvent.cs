using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class LevelChangedEvent : BaseEvent
	{
		/// <summary>Construct an instance.</summary>
		internal LevelChangedEvent() { }

		/// <summary>Hook this event to an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Hook(IModEvents listener)
		{
			listener.Player.LevelChanged += OnLevelChanged;
		}

		/// <summary>Unhook this event from an event listener.</summary>
		/// <param name="listener">Interface to the SMAPI event handler.</param>
		public override void Unhook(IModEvents listener)
		{
			listener.Player.LevelChanged -= OnLevelChanged;
		}

		/// <summary>Raised after a player's skill level changes.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnLevelChanged(object sender, LevelChangedEventArgs e)
		{
			// ensure immediate perks get removed on skill reset
			if (e.IsLocalPlayer && e.NewLevel == 0) LevelUpMenu.RevalidateHealth(e.Player);
		}
	}
}
