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
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetImmediateProfessionPerkPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.getImmediateProfessionPerk));
			Postfix = new(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkPostfix));
			Transpiler = new(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to add modded immediate profession perks.</summary>
		[HarmonyPostfix]
		private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
		{
			try
			{
				if (!Util.Professions.IndexByName.TryGetReverseValue(whichProfession, out var professionName)) return;

				// add immediate perks
				if (professionName == "Aquarist")
					foreach (var b in Game1.getFarm().buildings.Where(b =>
						(b.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
						b is FishPond && !b.isUnderConstruction()))
					{
						var pond = (FishPond) b;
						pond.UpdateMaximumOccupancy();
					}

				// initialize mod data, assets and helpers
				ModEntry.Data.InitializeDataFieldsForProfession(professionName);

				// subscribe events
				ModEntry.Subscriber.SubscribeEventsForProfession(professionName);

				if (whichProfession is >= 26 and < 30 &&
				    ModEntry.SuperModeIndex < 0) // is level 10 combat profession and super mode is not yet registered
					// register super mode
					ModEntry.SuperModeIndex = whichProfession;
			}
			catch (Exception ex)
			{
				Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(
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