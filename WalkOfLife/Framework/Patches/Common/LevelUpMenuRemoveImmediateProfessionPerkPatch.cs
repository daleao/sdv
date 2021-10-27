using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.removeImmediateProfessionPerk));
			Postfix = new(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkPostfix));
			Transpiler = new(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remove modded immediate profession perks.</summary>
		[HarmonyPostfix]
		private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
		{
			try
			{
				if (!Util.Professions.IndexByName.TryGetReverseValue(whichProfession, out var professionName)) return;

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
				if (!professionName.AnyOf("Scavenger", "Prospector"))
					ModEntry.Data.RemoveProfessionDataFields(professionName);

				// unsubscribe unnecessary events
				ModEntry.Subscriber.UnsubscribeProfessionEvents(professionName);

				// unregister super mode
				if (ModEntry.SuperModeIndex != whichProfession) return;

				var superModeProfessions = new[] {"Brute", "Poacher", "Desperado", "Piper"};
				if (Game1.player.HasAnyOfProfessions(superModeProfessions.Except(new[] {professionName}).ToArray()))
					ModEntry.SuperModeIndex = Util.Professions.IndexOf(superModeProfessions.First());
				else
					ModEntry.SuperModeIndex = -1;

				ModEntry.SuperModeCounter = 0;
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
					)
					.SetOperand(Util.Professions.IndexOf("Brute"));
			}
			catch (Exception ex)
			{
				Log($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}