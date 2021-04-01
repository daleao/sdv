namespace TheLion.AwesomeProfessions
{
	/// <summary>Interface for dynamic events.</summary>
	internal interface IEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		void Hook();

		/// <summary>Unhook this event from the event listener.</summary>
		void Unhook();
	}
}