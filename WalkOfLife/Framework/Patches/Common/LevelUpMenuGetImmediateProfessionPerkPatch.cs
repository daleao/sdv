using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuGetImmediateProfessionPerkPatch()
		{
			Original = typeof(LevelUpMenu).MethodNamed(nameof(LevelUpMenu.getImmediateProfessionPerk));
			Postfix = new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkPostfix));
			Transpiler = new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkTranspiler));
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
				{
					foreach (var b in Game1.getFarm().buildings.Where(b => (b.owner.Value == Game1.player.UniqueMultiplayerID || !Game1.IsMultiplayer) && b is FishPond && !b.isUnderConstruction()))
					{
						var pond = (FishPond)b;
						pond.UpdateMaximumOccupancy();
					}
				}

				// initialize mod data, assets and helpers
				ModEntry.Data.InitializeDataFieldsForProfession(whichProfession);

				// subscribe events
				ModEntry.Subscriber.SubscribeEventsForProfession(whichProfession);

				// register super mode
				var combatProfessions = new[] { "Brute", "Poacher", "Desperado", "Piper" };
				if (!professionName.AnyOf(combatProfessions) ||
					Game1.player.HasAnyOfProfessions(combatProfessions.Except(new[] { professionName }).ToArray())) return;

				ModEntry.SuperModeIndex = whichProfession;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
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
				Helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}