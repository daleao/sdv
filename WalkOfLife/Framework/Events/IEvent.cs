using StardewModdingAPI.Events;

namespace TheLion.AwesomeProfessions
{
	public interface IEvent
	{
		/// <summary>Hook this event to an event handler.</summary>
		/// <param name="handler">Interface to the SMAPI event handler.</param>
		public void Hook(IModEvents handler);

		/// <summary>Unhook this event from an event handler.</summary>
		/// <param name="handler">Interface to the SMAPI event handler.</param>
		public void Unhook(IModEvents handler);
	}
}
