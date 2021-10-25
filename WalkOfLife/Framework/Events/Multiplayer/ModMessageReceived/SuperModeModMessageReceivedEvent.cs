using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
	{
		/// <inheritdoc />
		public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModEntry.UniqueID) return;

			var key = e.ReadAs<int>();
			if (!ModEntry.ActivePeerSuperModes.ContainsKey(key))
				ModEntry.ActivePeerSuperModes[key] = new();

			switch (e.Type)
			{
				case "SuperModeActivated":
					ModEntry.ActivePeerSuperModes[key].Add(e.FromPlayerID);
					break;

				case "SuperModeDeactivated":
					ModEntry.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
					break;
			}
		}
	}
}