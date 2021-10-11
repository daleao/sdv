using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public abstract class ModMessageReceivedEvent : BaseEvent
	{
		/// <inheritdoc />
		public override void Hook()
		{
			ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		}

		/// <inheritdoc />
		public override void Unhook()
		{
			ModEntry.ModHelper.Events.Multiplayer.ModMessageReceived -= OnModMessageReceived;
		}

		/// <summary>Raised after a mod message is received over the network.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public abstract void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e);
	}
}