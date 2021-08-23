using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBuffsDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int BASE_SHEET_INDEX_OFFSET = 10;
		private const int SUPERMODE_SHEET_INDEX_OFFSET = BASE_SHEET_INDEX_OFFSET + 12;

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModEntry.SuperModeIndex < 0) ModEntry.Subscriber.Unsubscribe(GetType());

			var buffID = ModEntry.UniqueID.Hash() - ModEntry.SuperModeIndex;
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
						source: professionName,
						displaySource: ModEntry.I18n.Get(professionName.ToLower() + ".name"))
					{
						which = buffID,
						sheetIndex = professionIndex + BASE_SHEET_INDEX_OFFSET,
						millisecondsDuration = 49,
						description = ModEntry.I18n.Get(professionName.ToLower() + ".buffdesc",
							new { magnitude1 = GetSuperModePrimaryBuffMagnitude(professionName), magnitude2 = GetSuperModeSecondaryBuffMagnitude() })
					});
			}

			if (!ModEntry.IsSuperModeActive) return;

			buffID += 4;
			buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
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
						sheetIndex = professionIndex + SUPERMODE_SHEET_INDEX_OFFSET,
						millisecondsDuration = (int)(500f / 15f * 1000f),
						description = ModEntry.I18n.Get(professionName.ToLower() + ".supermdesc")
					}
				);
			}
		}

		/// <summary>Get the magnitude of the primary super mode buff for the given profession.</summary>
		/// <param name="professionName">A super mode profession.</param>
		private static string GetSuperModePrimaryBuffMagnitude(string professionName)
		{
			return (professionName switch
			{
				"Brute" => Util.Professions.GetBruteBonusDamageMultiplier(Game1.player) - 1f,
				"Hunter" => Util.Professions.GetHunterStealChance(Game1.player),
				"Desperado" => Util.Professions.GetDesperadoDoubleStrafeChance(),
				"Piper" => Util.Professions.GetPiperSlowChance(),
				_ => 0f
			} * 100f).ToString("0.0");
		}

		/// <summary>Get the magnitude of the secondary super mode buff for any profession.</summary>
		private static string GetSuperModeSecondaryBuffMagnitude()
		{
			return ((1f - Util.Professions.GetCooldownOrChargeTimeReduction()) * 100f).ToString("0.0");
		}

	}
}
