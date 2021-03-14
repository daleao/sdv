using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using TheLion.Common.Extensions;
using TheLion.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class Game1DrawHUDPatch : BasePatch
	{
		private static ILHelper _helper;

		private static readonly IEnumerable<int> _resourceNodeIds = new HashSet<int>
		{
			// ores
			751,	// copper node
			849,	// copper ?
			290,	// silver node
			850,	// silver ?
			764,	// gold node
			765,	// iridium node
			95,		// radioactive node

			// geodes
			75,		// geode node
			76,		// frozen geode node
			77,		// magma geode node
			819,	// omni geode node

			// gems
			44,		// gem node
			8,		// amethyst node
			10,		// topaz node
			12,		// emerald node
			14,		// aquamarine node
			6,		// jade node
			4,		// ruby node
			2,		// diamond node

			// other
			843,	// cinder shard node
			844,	// cinder shard node
			25,		// mussel node
			816,	// bone node
			817,	// bone node
			818,	// clay node
			46		// mystic stone
		};
		
		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal Game1DrawHUDPatch(ProfessionsConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(Game1), name: "drawHUD"),
				transpiler: new HarmonyMethod(GetType(), nameof(Game1DrawHUDTranspiler))
			);
		}

		/// <summary>Patch for Scavenger to track forageables indoors.</summary>
		protected static IEnumerable<CodeInstruction> Game1DrawHUDTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(Game1)}::drawHUD.");

			/// From: if (!player.professions.Contains(<scavenger_id>) || !currentLocation.IsOutdoors) return
			/// To: if (!(player.professions.Contains(<scavenger_id>) || player.professions.Contains(<prospector_id>)) return

			Label isProspector = iLGenerator.DefineLabel();
			try
			{
				_helper
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
					.SetOperand(Utils.ProfessionMap.Forward["prospector"])				// change to prospector check
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
			catch(Exception ex)
			{
				_helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			/// From: if ((bool)pair.Value.isSpawnedObject || pair.Value.ParentSheetIndex == 590) ...
			/// To: if (_ShouldDraw(pair.Value)) ...

			try
			{
				_helper
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
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Game1DrawHUDPatch), nameof(_ShouldDraw))),
						new CodeInstruction(OpCodes.Brfalse, operand: loopHead)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching modded tracking pointers draw condition. Helper returned {ex}").Restore();
			}

			return _helper.Flush();
		}

		/// <summary>Whether the game should draw an arrow over a given object.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _ShouldDraw(SObject obj)
		{
			return (Utils.LocalPlayerHasProfession("scavenger") && ((obj.IsSpawnedObject && !_IsForagedMineral(obj)) || obj.ParentSheetIndex == 590))
				|| (Utils.LocalPlayerHasProfession("prospector") && (_IsResourceNode(obj) || _IsForagedMineral(obj)));
		}

		/// <summary>Whether a given object is a resource node or foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsResourceNode(SObject obj)
		{
			return _resourceNodeIds.Contains(obj.ParentSheetIndex);
		}

		/// <summary>Whether a given object is a foraged mineral.</summary>
		/// <param name="obj">The given object.</param>
		private static bool _IsForagedMineral(SObject obj)
		{
			return obj.Name.AnyOf("Quartz", "Earth Crystal", "Frozen Tear", "Fire Quartz");
		}
	}
}
