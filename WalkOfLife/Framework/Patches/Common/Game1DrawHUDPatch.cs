using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class Game1DrawHUDPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal Game1DrawHUDPatch()
		{
			Original = typeof(Game1).MethodNamed("drawHUD");
			Postfix = new HarmonyMethod(GetType(), nameof(Game1DrawHUDPostfix));
			Transpiler = new HarmonyMethod(GetType(), nameof(Game1DrawHUDTranspiler));
		}

		#region harmony patches

		/// <summary>Patch for Prospector to track ladders and shafts.</summary>
		[HarmonyPostfix]
		private static void Game1DrawHUDPostfix()
		{
			try
			{
				if (!Game1.player.HasProfession("Prospector") || Game1.currentLocation is not MineShaft shaft) return;
				foreach (var tile in Util.Tiles.GetLadderTiles(shaft))
					Util.HUD.DrawTrackingArrowPointer(tile, Color.Lime);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// From: if (!player.professions.Contains(<scavenger_id>) || !currentLocation.IsOutdoors) return
			/// To: if (!(player.professions.Contains(<scavenger_id>) || player.professions.Contains(<prospector_id>)) return

			var isProspector = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindProfessionCheck(Farmer.tracker) // find index of tracker check
					.Retreat()
					.ToBufferUntil(
						new CodeInstruction(OpCodes.Brfalse) // copy profession check
					)
					.InsertBuffer() // paste
					.Return()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_S)
					)
					.SetOperand(Util.Professions.IndexOf("Prospector")) // change to prospector check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Brtrue_S, isProspector) // change !(A && B) to !(A || B)
					)
					.Advance()
					.StripLabels() // strip repeated label
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call,
							typeof(Game1).PropertyGetter(nameof(Game1.currentLocation)))
					)
					.Remove(3) // remove currentLocation.IsOutdoors check
					.AddLabels(isProspector); // branch here is first profession check was true
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}");
				return null;
			}

			/// From: if ((bool)pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) ...
			/// To: if (_ShouldDraw(pair.Value)) ...

			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Bne_Un) // find branch to loop head
					)
					.GetOperand(out var loopHead) // copy destination
					.RetreatUntil(
#pragma warning disable AvoidNetField // Avoid Netcode types when possible
						new CodeInstruction(OpCodes.Ldfld,
							typeof(SObject).Field(nameof(SObject.isSpawnedObject)))
#pragma warning restore AvoidNetField // Avoid Netcode types when possible
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes
							.Bne_Un) // remove pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590
					)
					.Insert( // insert call to custom condition
						new CodeInstruction(OpCodes.Call,
							typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.ShouldBeTracked))),
						new CodeInstruction(OpCodes.Brfalse, loopHead)
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}