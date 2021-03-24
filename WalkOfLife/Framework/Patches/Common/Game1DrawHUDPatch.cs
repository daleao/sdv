using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class Game1DrawHUDPatch : BasePatch
	{
		private static ILHelper _Helper { get; set; }

		/// <summary>Construct an instance.</summary>
		internal Game1DrawHUDPatch()
		{
			_Helper = new ILHelper(Monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), name: "drawHUD"),
				transpiler: new HarmonyMethod(GetType(), nameof(Game1DrawHUDTranspiler)),
				postfix: new HarmonyMethod(GetType(), nameof(Game1DrawHUDPostfix))
			);
		}

		#region harmony patches
		/// <summary>Patch for Scavenger and Prospector to track different stuff.</summary>
		protected static IEnumerable<CodeInstruction> Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_Helper.Attach(instructions).Log($"Patching method {typeof(Game1)}::drawHUD.");

			/// From: if (!player.professions.Contains(<scavenger_id>) || !currentLocation.IsOutdoors) return
			/// To: if (!(player.professions.Contains(<scavenger_id>) || player.professions.Contains(<prospector_id>)) return

			Label isProspector = iLGenerator.DefineLabel();
			try
			{
				_Helper
					.FindProfessionCheck(Farmer.tracker)								// find index of tracker check
					.Retreat()
					.ToBufferUntil(
						new CodeInstruction(OpCodes.Brfalse)							// copy profession check
					)
					.InsertBuffer()														// paste
					.Return()
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_S)
					)
					.SetOperand(Utility.ProfessionMap.Forward["prospector"])			// change to prospector check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Brfalse)
					)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Brtrue_S, operand: isProspector)	// change !(A && B) to !(A || B)
					)
					.Advance()
					.StripLabels()														// strip repeated label
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(Game1), nameof(Game1.currentLocation)).GetGetMethod())
					)
					.Remove(3)															// remove currentLocation.IsOutdoors check
					.AddLabel(isProspector);											// branch here is first profession check was true
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			_Helper.Backup();

			/// From: if ((bool)pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) ...
			/// To: if (_ShouldDraw(pair.Value)) ...

			try
			{
				_Helper
					.FindNext(
						new CodeInstruction(OpCodes.Bne_Un)	// find branch to loop head
					)
					.GetOperand(out object loopHead)		// copy destination
					.RetreatUntil(
						#pragma warning disable AvoidNetField // Avoid Netcode types when possible
						new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SObject), nameof(SObject.isSpawnedObject)))
						#pragma warning restore AvoidNetField // Avoid Netcode types when possible
					)
					.RemoveUntil(
						new CodeInstruction(OpCodes.Bne_Un)	// remove pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590
					)
					.Insert(								// insert call to custom condition
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.ShouldPlayerTrackObject))),
						new CodeInstruction(OpCodes.Brfalse, operand: loopHead)
					);
			}
			catch (Exception ex)
			{
				_Helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			return _Helper.Flush();
		}

		/// <summary>Patch for Prospector to track initial ladder down + draw ticks over trackable objects in view.</summary>
		protected static void Game1DrawHUDPostfix()
		{
			// track initial ladder down
			if (AwesomeProfessions.initialLadderTiles.Count() > 0)
			{
				foreach (var tile in AwesomeProfessions.initialLadderTiles)
					Utility.DrawTrackingArrowPointer(tile, Color.Lime);
			}

			//if (!AwesomeProfessions.Config.Modkey.IsDown()) return;

			//// draw ticks over trackable objects in view
			//foreach (var kvp in Game1.currentLocation.Objects.Pairs)
			//	if (Utility.ShouldPlayerTrackObject(kvp.Value)) Utility.DrawArrowPointerOverTarget(kvp.Key, Color.Yellow);
		}
		#endregion harmony patches
	}
}
