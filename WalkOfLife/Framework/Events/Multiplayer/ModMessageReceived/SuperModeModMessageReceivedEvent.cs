using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
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
					var glowingColor = Utility.Professions.NameOf(key) switch
					{
						"Brute" => Color.OrangeRed,
						"Poacher" => Color.MediumPurple,
						"Desperado" => Color.DarkGoldenrod,
						"Piper" => Color.LimeGreen,
						_ => Color.White
					};
					Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
					break;

				case "SuperModeDeactivated":
					ModEntry.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
					Game1.getFarmer(e.FromPlayerID).stopGlowing();
					break;
			}
		}
	}
}