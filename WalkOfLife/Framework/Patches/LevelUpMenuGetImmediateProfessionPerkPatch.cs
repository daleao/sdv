using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal LevelUpMenuGetImmediateProfessionPerkPatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.getImmediateProfessionPerk)),
				transpiler: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(LevelUpMenuGetImmediateProfessionPerkPostfix))
			);
		}

		/// <summary>Patch to move bonus health from Defender to Brute.</summary>
		protected static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(LevelUpMenu)}::{nameof(LevelUpMenu.getImmediateProfessionPerk)}.");

			/// From: case <defender_id>:
			/// To: case <brute_id>:

			try
			{
				_helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 27)
					)
					.SetOperand(Utils.ProfessionMap.Forward["brute"]);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Patch to add modded immediate profession perks.</summary>
		protected static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
		{
			if (whichProfession == Utils.ProfessionMap.Forward["angler"])
			{
				FishingRod.maxTackleUses *= 2;
			}

			if (whichProfession == Utils.ProfessionMap.Forward["aquarist"])
			{
				foreach (Building b in Game1.getFarm().buildings)
				{
					if ((b.owner.Equals(Game1.player.UniqueMultiplayerID) || !Game1.IsMultiplayer) && b is FishPond)
					{
						(b as FishPond).UpdateMaximumOccupancy();
					}
				}
			}
		}
	}
}
