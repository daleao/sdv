using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getImmediateProfessionPerk)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.getImmediateProfessionPerk)}.");

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
			if (!Utility.ProfessionMap.TryGetReverseValue(whichProfession, out string professionName)) return;

			// add immediate perks
			if (professionName.Equals("Angler")) FishingRod.maxTackleUses = 40;
			else if (professionName.Equals("Aquarist"))
			{
				foreach (Building b in Game1.getFarm().buildings)
				{
					if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond)
						(b as FishPond).UpdateMaximumOccupancy();
				}
			}

			// initialize mod data, assets and helpers
			Utility.InitializeModData(whichProfession);

			// subscribe events
			AwesomeProfessions.EventManager.SubscribeEventsForProfession(whichProfession);
		}

		#endregion harmony patches
	}
}