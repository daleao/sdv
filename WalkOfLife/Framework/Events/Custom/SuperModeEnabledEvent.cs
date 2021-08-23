using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeEnabledEventHandler();

	public class SuperModeEnabledEvent : BaseEvent
	{
		private readonly SuperModeCountdownUpdateTickedEvent _superModeCountdownUpdateTickedEvent = new();

		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeEnabled += OnSuperModeEnabled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeEnabled -= OnSuperModeEnabled;
		}

		/// <summary>Raised when IsSuperModeActive is set to true.</summary>
		public void OnSuperModeEnabled()
		{
			ModEntry.Subscriber.Subscribe(_superModeCountdownUpdateTickedEvent);

			var whichSuperMode = Util.Professions.NameOf(ModEntry.SuperModeIndex);
			switch (whichSuperMode)
			{
				case "Brute":
					Game1.currentLocation.playSound("wand");
					Game1.player.startGlowing(Color.OrangeRed, border: false, 0.05f);
					break;
				case "Hunter":
					Game1.currentLocation.playSound("cast");
					Game1.player.startGlowing(Color.GhostWhite, border: false, 0.05f);
					Game1.player.glowingTransparency = 0.1f;
					break;
				case "Desperado":
					Game1.currentLocation.playSound("powerup");
					Game1.player.startGlowing(Color.DarkGoldenrod, border: false, 0.05f);
					break;
				case "Piper":
					Game1.currentLocation.playSound("powerup");
					Game1.player.startGlowing(Color.LightSeaGreen, border: false, 0.05f);

					var location = Game1.currentLocation;
					var slimes = from npc in location.characters.OfType<GreenSlime>() select npc;
					var r = new Random(Guid.NewGuid().GetHashCode());
					foreach (var slime in slimes)
					{
						// enrage
						if (slime.cute.Value && !slime.focusedOnFarmers)
						{
							slime.DamageToFarmer += slime.DamageToFarmer / 2;
							slime.shake(1000);
							slime.focusedOnFarmers = true;
						}

						if (Game1.random.NextDouble() > 0.25) return;

						// try to make special
						slime.hasSpecialItem.Value = true;
						slime.Health *= 3;
						slime.DamageToFarmer *= 2;
					}

					break;
			}

			ModEntry.Multiplayer.SendMessage(message: ModEntry.SuperModeIndex, messageType: "SuperModeActivated", modIDs: new[] { ModEntry.UniqueID });
		}
	}
}
