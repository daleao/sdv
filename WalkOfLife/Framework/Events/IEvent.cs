namespace TheLion.Stardew.Professions.Framework.Events
{
	/// <summary>Interface for dynamic events.</summary>
	internal interface IEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public void Hook();

		/// <summary>Unhook this event from the event listener.</summary>
		public void Unhook();
	}
}