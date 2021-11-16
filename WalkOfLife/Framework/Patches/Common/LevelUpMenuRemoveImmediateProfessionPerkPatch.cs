using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
		{
			Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
		}

		#region harmony patches

		/// <summary>Patch to remove modded immediate profession perks.</summary>
		[HarmonyPostfix]
		private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
		{
			if (!Utility.Professions.IndexByName.TryGetReverseValue(whichProfession, out var professionName)) return;

			// remove immediate perks
			if (professionName == "Aquarist")
				foreach (var b in Game1.getFarm().buildings.Where(b =>
					(b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
					b is FishPond && !b.isUnderConstruction() && b.maxOccupants.Value > 10))
				{
					b.maxOccupants.Set(10);
					b.currentOccupants.Value = Math.Min(b.currentOccupants.Value, b.maxOccupants.Value);
				}

			// clean unnecessary mod data
			if (!professionName.IsAnyOf("Scavenger", "Prospector"))
				ModEntry.Data.RemoveProfessionData(professionName);

			// unsubscribe unnecessary events
			ModEntry.Subscriber.UnsubscribeProfessionEvents(professionName);

			// unregister Super Mode
			if (ModState.SuperModeIndex != whichProfession) return;

			var otherSuperModeProfessions = new[] {"Brute", "Poacher", "Desperado", "Piper"}
				.Except(new[] {professionName}).ToArray();
			var x = Game1.player.professions;
			if (Game1.player.HasAnyOfProfessions(otherSuperModeProfessions, out var firstMatch))
				ModState.SuperModeIndex = Utility.Professions.IndexOf(firstMatch);
			else
				ModState.SuperModeIndex = -1;

			ModState.SuperModeGaugeValue = 0;
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
					)
					.SetOperand(Utility.Professions.IndexOf("Brute"));
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}