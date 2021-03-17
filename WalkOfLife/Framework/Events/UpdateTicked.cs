using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Linq;

namespace TheLion.AwesomeProfessions
{
	public partial class AwesomeProfessions
	{
		public static int DemolitionistBuffMagnitude { get; set; } = 0;
		public static uint BruteKillStreak { get; set; } = 0;

		/// <summary>Raised after the game state is updated.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			// handle Spelunker buff
			if (Utility.LocalPlayerHasProfession("spelunker") && Game1.currentLocation is MineShaft)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.SpelunkerBuffID);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: 1, 0, 0, minutesDuration: 1, source: "spelunker", displaySource: Helper.Translation.Get("spelunker.name"))
						{
							which = Utility.SpelunkerBuffID,
						}
					);
				}
			}

			// handle Demolitionist buff
			if (DemolitionistBuffMagnitude > 0)
			{
				if (e.Ticks % 30 == 0)
				{
					int buffDecay = DemolitionistBuffMagnitude > 4 ? 2 : 1;
					DemolitionistBuffMagnitude = Math.Max(0, DemolitionistBuffMagnitude - buffDecay);
				}

				int buffId = Utility.DemolitionistBuffID + DemolitionistBuffMagnitude;
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
				if (buff == null)
				{
					Game1.buffsDisplay.addOtherBuff(
						buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, speed: DemolitionistBuffMagnitude, 0, 0, minutesDuration: 1, source: "demolitionist", displaySource: Helper.Translation.Get("demolitionist.name"))
						{
							which = buffId,
							millisecondsDuration = 50,
							description = Helper.Translation.Get("demolitionist.buffdescription")
						}
					);
				}
			}

			// handle Brute buff
			if (BruteKillStreak > 0)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.BruteBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(Utility.BruteBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "brute", displaySource: Helper.Translation.Get("brute.name"))
					{
						which = Utility.BruteBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("brute.buffdescription", new { buffMagnitude = Math.Truncate(BruteKillStreak * 5.0) / 10 })
					}
				);
			}

			// handle Gambit buff
			double healthPercent;
			if (Utility.LocalPlayerHasProfession("gambit") && (healthPercent = (double)Game1.player.health / Game1.player.maxHealth) < 1)
			{
				Buff buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == Utility.GambitBuffID);
				if (buff != null)
					Game1.buffsDisplay.removeOtherBuff(Utility.GambitBuffID);

				Game1.buffsDisplay.addOtherBuff(
					buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, minutesDuration: 1, source: "gambit", displaySource: Helper.Translation.Get("gambit.name"))
					{
						which = Utility.GambitBuffID,
						sheetIndex = 20,
						millisecondsDuration = 50,
						description = Helper.Translation.Get("gambit.buffdescription", new { buffMagnitude = Math.Truncate(200.0 / (healthPercent + 0.2) - 200.0 / 1.2) / 10 })
					}
				);
			}
		}
	}
}
