using Harmony;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions
{
	internal class LevelUpMenuRemoveImmediateProfessionPerkPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		internal LevelUpMenuRemoveImmediateProfessionPerkPatch()
		{
			_helper = new ILHelper(_monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.removeImmediateProfessionPerk)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuRemoveImmediateProfessionPerkPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		protected static IEnumerable<CodeInstruction> LevelUpMenuRemoveImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.removeImmediateProfessionPerk)}.");

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				_helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 27)
					)
					.SetOperand(Utility.ProfessionMap.Forward["brute"]);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch to remove modded immediate profession perks.</summary>
		protected static void LevelUpMenuRemoveImmediateProfessionPerkPostfix(int whichProfession)
		{
			if (whichProfession == Utility.ProfessionMap.Forward["angler"]) FishingRod.maxTackleUses = 20;
			else if (whichProfession == Utility.ProfessionMap.Forward["aquarist"])
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

			AwesomeProfessions.EventManager.UnsubscribeEventsForProfession(whichProfession);
		}
		#endregion harmony patches
	}
}
