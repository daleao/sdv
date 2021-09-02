using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeEnabledEventHandler();

	public class SuperModeEnabledEvent : BaseEvent
	{
		private const int SHEET_INDEX_OFFSET = 22;

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
			// remove shake timer
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarShakeTimerUpdateTickedEvent));
			ModEntry.ShouldShakeSuperModeBar = false;

			// add countdown event
			ModEntry.Subscriber.Subscribe(new SuperModeCountdownUpdateTickedEvent());

			// enable corresponding super mode
			SoundEffect sfx = null;
			var whichSuperMode = Util.Professions.NameOf(ModEntry.SuperModeIndex);
			switch (whichSuperMode)
			{
				case "Brute":
					if (ModEntry.SfxLoader.SfxByName.TryGetValue("brute_rage", out sfx))
						sfx.CreateInstance().Play();
					Game1.player.startGlowing(Color.OrangeRed, border: false, 0.05f);
					break;
				case "Hunter":
					if (ModEntry.SfxLoader.SfxByName.TryGetValue("hunter_invis", out sfx))
						sfx.CreateInstance().Play();
					Game1.player.startGlowing(Color.GhostWhite, border: false, 0.05f);
					break;
				case "Desperado":
					if (ModEntry.SfxLoader.SfxByName.TryGetValue("", out sfx))
						sfx.CreateInstance().Play();
					Game1.player.startGlowing(Color.DarkGoldenrod, border: false, 0.05f);
					break;
				case "Piper":
					if (ModEntry.SfxLoader.SfxByName.TryGetValue("piper_provoke", out sfx))
						sfx.CreateInstance().Play();
					Game1.player.startGlowing(Color.LightSeaGreen, border: false, 0.05f);

					// do enrage and mutations
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

			// display buff
			var buffID = ModEntry.UniqueID.Hash() + ModEntry.SuperModeIndex + 4;
			var professionIndex = ModEntry.SuperModeIndex;
			var professionName = Util.Professions.NameOf(professionIndex);
			
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
			if (buff == null)
			{
				Game1.buffsDisplay.addOtherBuff(
					new Buff(0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						minutesDuration: 1,
						source: "SuperMode",
						displaySource: ModEntry.I18n.Get(professionName.ToLower() + ".superm"))
					{
						which = buffID,
						sheetIndex = professionIndex + SHEET_INDEX_OFFSET,
						millisecondsDuration = (int)(ModEntry.Config.SuperModeDrainFactor / 60f * ModEntry.SuperModeCounterMax * 1000f),
						description = ModEntry.I18n.Get(professionName.ToLower() + ".supermdesc")
					}
				);
			}

			// notify peers
			ModEntry.Multiplayer.SendMessage(message: ModEntry.SuperModeIndex, messageType: "SuperModeActivated", modIDs: new[] { ModEntry.UniqueID });
		}
	}
}
