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
	internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
	{
		/// <inheritdoc/>
		public override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.removeImmediateProfessionPerk)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkPostfix))
			);
		}

		#region harmony patches

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		private static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			Helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.removeImmediateProfessionPerk)}.");

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 27)
					)
					.SetOperand(Utility.ProfessionMap.Forward["Brute"]);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return Helper.Flush();
		}

		/// <summary>Patch to remove modded immediate profession perks.</summary>
		private static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
		{
			if (!Utility.ProfessionMap.TryGetReverseValue(whichProfession, out string professionName)) return;

			// remove immediate perks
			if (professionName.Equals("Angler")) FishingRod.maxTackleUses = 20;
			else if (professionName.Equals("Aquarist"))
			{
				foreach (Building b in Game1.getFarm().buildings)
				{
					if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond && b.maxOccupants.Value > 10)
					{
						b.maxOccupants.Set(10);
						b.currentOccupants.Value = Math.Min(b.currentOccupants.Value, b.maxOccupants.Value);
					}
				}
			}

			// clean unnecessary mod data
			Utility.CleanModData(whichProfession);

			// unsubscribe unnecessary events
			AwesomeProfessions.EventManager.UnsubscribeProfessionEvents(whichProfession);
		}

		#endregion harmony patches
	}
}