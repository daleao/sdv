using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBuffDisplayUpdateTickedEvent : UpdateTickedEvent
	{
		private const int SHEET_INDEX_OFFSET = 10;

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModEntry.SuperModeIndex <= 0) ModEntry.Subscriber.Unsubscribe(GetType());

			var buffID = ModEntry.UniqueID.Hash() + ModEntry.SuperModeIndex;
			var professionIndex = ModEntry.SuperModeIndex;
			var professionName = Util.Professions.NameOf(professionIndex);

			var magnitude1 = GetSuperModePrimaryBuffMagnitude(professionName);
			if (Math.Abs(float.Parse(magnitude1)) < 0.1f) return;

			var magnitude2 = GetSuperModeSecondaryBuffMagnitude(professionName);

			var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
			if (buff == null)
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
						1,
						professionName,
						ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".name." +
						                                   (Game1.player.IsMale ? "male" : "female")))
					{
						which = buffID,
						sheetIndex = professionIndex + SHEET_INDEX_OFFSET,
						millisecondsDuration = 50,
						description = ModEntry.ModHelper.Translation.Get(professionName.ToLower() + ".buffdesc",
							new {magnitude1, magnitude2})
					});
		}

		/// <summary>Get the magnitude of the primary super mode buff for the given profession.</summary>
		/// <param name="professionName">A super mode profession.</param>
		private static string GetSuperModePrimaryBuffMagnitude(string professionName)
		{
			return professionName switch
			{
				"Brute" => ((Util.Professions.GetBruteBonusDamageMultiplier(Game1.player) - 1.15f) * 100f)
					.ToString("0.0"),
				"Poacher" => (Util.Professions.GetPoacherStealChance(Game1.player) * 100f).ToString("0.0"),
				"Desperado" => (Util.Professions.GetDesperadoDoubleStrafeChance() * 100f).ToString("0.0"),
				"Piper" => Util.Professions.GetPiperSlimeSpawnAttempts().ToString("0"),
				_ => throw new ArgumentException($"Unexpected profession name {professionName}")
			};
		}

		/// <summary>Get the magnitude of the secondary super mode buff for the given profession.</summary>
		/// <param name="professionName">A super mode profession.</param>
		private static string GetSuperModeSecondaryBuffMagnitude(string professionName)
		{
			return professionName == "Piper"
				? (Util.Professions.GetPiperSlimeAttackSpeedModifier() * 100f).ToString("0.0")
				: ((1f - Util.Professions.GetCooldownOrChargeTimeReduction()) * 100f).ToString("0.0");
		}
	}
}