using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Harmony;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal class FarmAnimalDayUpdatePatch : BasePatch
	{
		private static ILHelper _helper;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal FarmAnimalDayUpdatePatch(ModConfig config, IMonitor monitor)
		: base(config, monitor)
		{
			_helper = new ILHelper(monitor);
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal override void Apply(HarmonyInstance harmony)
		{
			harmony.Patch(
				AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.dayUpdate)),
				transpiler: new HarmonyMethod(GetType(), nameof(FarmAnimalDayUpdateTranspiler))
			);
		}

		/// <summary>Patch for Producer to double produce frequency at max animal happiness + combine shepherd and coopmaster product quality boosts.</summary>
		protected static IEnumerable<CodeInstruction> FarmAnimalDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(FarmAnimal)}::{nameof(FarmAnimal.dayUpdate)}.");

			/// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
			/// To: FarmAnimal.daysToLay /= (FarmAnimal.happiness.Value >= 200) && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>) ? 2 : 1

			try
			{
				_helper
					.Find(													// find index of FarmAnimal.type.Value.Equals("Sheep")
						new CodeInstruction(OpCodes.Ldstr, operand: "Sheep"),
						new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(string), nameof(string.Equals), new Type[] { typeof(string) }))
					)
					.Retreat(2)
					.SetOperand(AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.happiness)))													// was FarmAnimal.type
					.Advance()
					.SetOperand(AccessTools.Property(typeof(NetFieldBase<byte, NetByte>), nameof(NetFieldBase<byte, NetByte>.Value)).GetGetMethod())	// was <string, NetString>
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Ldc_I4_S, operand: 200)	// was Ldstr "Sheep"
					)
					.Advance()
					.Remove()
					.SetOpCode(OpCodes.Blt_S)								// was Brfalse
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_0)
					)
					.SetOpCode(OpCodes.Ldc_I4_1)							// was Ldc_I4_0
					.Advance(2)
					.SetOpCode(OpCodes.Ldc_I4_2)							// was Ldc_I4_1
					.Advance()
					.SetOpCode(OpCodes.Div)									// was Sub
					.Advance()
					.Insert(
						new CodeInstruction(OpCodes.Conv_U1)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Producer produce frequency.\nHelper returned {ex}").Restore();
			}

			_helper.Backup();

			/// From: if ((!isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<shepherd_id>)) || (isCoopDweller() && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<coopmaster_id>)))
			/// To: if (Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>)

			try
			{
				_helper
					.Find(									// find index of first FarmAnimal.isCoopDweller check
						fromCurrentIndex: true,
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.isCoopDweller)))
					)
					.Retreat()
					.Remove(3)								// remove this check
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.isCoopDweller)))	// second FarmAnimal.isCoopDweller
					)
					.Advance()								// the branch to resume execution
					.GetOperand(out object isNotProducer)	// copy destination
					.Retreat(2)
					.Insert(								// branch to skip this check if player is not producer
						new CodeInstruction(OpCodes.Br_S, operand: (Label)isNotProducer)
					);
			}
			catch (Exception ex)
			{
				_helper.Error($"Failed while patching Producer produce quality.\nHelper returned {ex}").Restore();
			}

			return _helper.Flush();
		}
	}
}
