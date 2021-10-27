using System;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBuffDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int
			SHEET_INDEX_OFFSET =
				10; // added to profession index to obtain the buff sheet index.</summary>

		/// <inheritdoc />
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModEntry.SuperModeIndex <= 0)
			{
				ModEntry.Subscriber.Unsubscribe(GetType());
				return;
			}

			if (ModEntry.SuperModeCounter < 10) return;

			var buffID = ModEntry.UniqueID.Hash() + ModEntry.SuperModeIndex;
			var professionIndex = ModEntry.SuperModeIndex;
			var professionName = Util.Professions.NameOf(professionIndex);
			var magnitude1 = GetSuperModePrimaryBuffMagnitude(professionName);
			var magnitude2 = GetSuperModeSecondaryBuffMagnitude(professionName);
			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
			if (buff == null)
				Game1.buffsDisplay.addOtherBuff(
					new(0,
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
						1,
						professionName,
						ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buff"))
					{
						which = buffID,
						sheetIndex = professionIndex + SHEET_INDEX_OFFSET,
						millisecondsDuration = 0,
						description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buffdesc",
							new {magnitude1, magnitude2})
					});
		}

		private static string GetSuperModePrimaryBuffMagnitude(string professionName)
		{
			return professionName switch
			{
				"Brute" => ((Util.Professions.GetBruteBonusDamageMultiplier(Game1.player) - 1.15f) * 100f)
					.ToString("0.0"),
				"Poacher" => Util.Professions.GetPoacherCritDamageMultiplier().ToString("0.0"),
				"Desperado" => ((Util.Professions.GetDesperadoBulletPower() - 1f) * 100f).ToString("0.0"),
				"Piper" => Util.Professions.GetPiperSlimeSpawnAttempts().ToString("0"),
				_ => throw new ArgumentException($"Unexpected profession name {professionName}")
			};
		}

		private static string GetSuperModeSecondaryBuffMagnitude(string professionName)
		{
			return professionName == "Piper"
				? (Util.Professions.GetPiperSlimeAttackSpeedModifier() * 100f).ToString("0.0")
				: ((1f - Util.Professions.GetCooldownOrChargeTimeReduction()) * 100f).ToString("0.0");
		}
	}
}