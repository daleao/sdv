using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TheLion.Common;

namespace TheLion.AwesomeTools
{
	/// <summary>Patches the game code to implement modded tool behavior.</summary>
	internal static class HarmonyPatcher
	{
		private static readonly List<int> _axeAffectedTilesRadii = AwesomeTools.Config.AxeConfig.RadiusAtEachPowerLevel;
		private static readonly List<int> _pickaxeAffectedTilesRadii = AwesomeTools.Config.PickaxeConfig.RadiusAtEachPowerLevel;

		// Enable Axe power level increase
		[HarmonyPatch(typeof(Axe), "beginUsing")]
		internal class Before_Axe_BeginUsing
		{
			protected static bool Prefix(ref Tool __instance, Farmer who)
			{
				if (!Utility.ShouldCharge(__instance))
					return true; // run original logic

				who.Halt();
				__instance.Update(who.FacingDirection, 0, who);
				switch (who.FacingDirection)
				{
					case 0:
						who.FarmerSprite.setCurrentFrame(176);
						__instance.Update(0, 0, who);
						break;

					case 1:
						who.FarmerSprite.setCurrentFrame(168);
						__instance.Update(1, 0, who);
						break;

					case 2:
						who.FarmerSprite.setCurrentFrame(160);
						__instance.Update(2, 0, who);
						break;

					case 3:
						who.FarmerSprite.setCurrentFrame(184);
						__instance.Update(3, 0, who);
						break;
				}

				return false; // don't run original logic
			}
		}

		// Enable Pickaxe power level increase
		[HarmonyPatch(typeof(Pickaxe), "beginUsing")]
		internal class Before_Pickaxe_BeginUsing
		{
			protected static bool Prefix(ref Tool __instance, Farmer who)
			{
				if (!Utility.ShouldCharge(__instance))
					return true; // run original logic

				who.Halt();
				__instance.Update(who.FacingDirection, 0, who);
				switch (who.FacingDirection)
				{
					case 0:
						who.FarmerSprite.setCurrentFrame(176);
						__instance.Update(0, 0, who);
						break;

					case 1:
						who.FarmerSprite.setCurrentFrame(168);
						__instance.Update(1, 0, who);
						break;

					case 2:
						who.FarmerSprite.setCurrentFrame(160);
						__instance.Update(2, 0, who);
						break;

					case 3:
						who.FarmerSprite.setCurrentFrame(184);
						__instance.Update(3, 0, who);
						break;
				}

				return false; // don't run original logic
			}
		}

		// Allow first two power levels on Pickaxe
		[HarmonyPatch(typeof(Farmer), "toolPowerIncrease")]
		internal class During_Farmer_ToolPowerIncrease
		{
			protected static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var l = instructions.ToList();
				for (int i = 0; i < instructions.Count(); ++i)
				{
					if (l[i].opcode == OpCodes.Isinst && l[i].operand.ToString().Equals("StardewValley.Tools.Pickaxe"))
					{
						// inject logic: branch over toolPower += 2
						l.Insert(i - 2, new CodeInstruction(OpCodes.Br_S, l[i + 1].operand));
						break;
					}
				}

				return l.AsEnumerable();
			}
		}

		// Set affected tiles for Axe and Pickaxe power levels
		[HarmonyPatch(typeof(Tool), "tilesAffected")]
		internal class After_Tool_TileseAffected
		{
			protected static void Postfix(ref Tool __instance, ref List<Vector2> __result, Vector2 tileLocation, int power, Farmer who)
			{
				if (__instance.UpgradeLevel < Tool.copper)
					return;

				if (__instance is Axe || __instance is Pickaxe)
				{
					__result.Clear();

					int radius = __instance is Axe ? _axeAffectedTilesRadii[Math.Min(power - 2, 4)] : _pickaxeAffectedTilesRadii[Math.Min(power - 2, 4)];
					if (radius == 0)
						return;

					CircleTileGrid grid = new CircleTileGrid(tileLocation, radius);
					__result.AddRange(grid);
				}
			}
		}

		// Hide affected tiles overlay for Axe or Pickaxe
		[HarmonyPatch(typeof(Tool), "draw")]
		internal class Before_Tool_Draw
		{
			protected static bool Prefix(ref Tool __instance)
			{
				return (__instance is not Axe || AwesomeTools.Config.AxeConfig.ShowAxeAffectedTiles) && (__instance is not Pickaxe || AwesomeTools.Config.PickaxeConfig.ShowPickaxeAffectedTiles);
			}
		}
	}
}