using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getImmediateProfessionPerk)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Trace($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.getImmediateProfessionPerk)}.");

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: Farmer.defender)
					)
					.SetOperand(Utility.ProfessionMap.Forward["Brute"]);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		/// <summary>Patch to add modded immediate profession perks.</summary>
		private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
		{
			try
			{
				if (!Utility.ProfessionMap.TryGetReverseValue(whichProfession, out var professionName)) return;

				// add immediate perks
				if (professionName.Equals("Aquarist"))
				{
					foreach (var b in Game1.getFarm().buildings.Where(b => (b.owner.Value.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && !b.isUnderConstruction()))
					{
						var pond = (FishPond)b;
						pond.UpdateMaximumOccupancy();
					}
				}

				// initialize mod data, assets and helpers
				Utility.InitializeModData(whichProfession);

				// subscribe events
				AwesomeProfessions.EventManager.SubscribeEventsForProfession(whichProfession);
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(LevelUpMenuGetImmediateProfessionPerkPostfix)}:\n{ex}");
			}
		}

		#endregion harmony patches
	}
}