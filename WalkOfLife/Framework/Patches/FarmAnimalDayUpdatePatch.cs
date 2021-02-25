using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TheLion.Common.Classes.Harmony;

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

		/// <summary>Patch for Producer to double produce frequency at max animal friendship + combine shepherd and coopmaster product quality boosts.</summary>
		protected static IEnumerable<CodeInstruction> FarmAnimalDayUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			_helper.Attach(instructions).Log($"Patching method {typeof(FarmAnimal)}::{nameof(FarmAnimal.dayUpdate)}.");

			/// From: FarmeAnimal.daysToLay -= (FarmAnimal.type.Value.Equals("Sheep") && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(Farmer.shepherd)) ? 1 : 0
			/// To: FarmAnimal.daysToLay /= (FarmAnimal.friendshipTowardsFarmer.Value >= 1000) && Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>) ? 2 : 1

			try
			{
				_helper
					.Find(									// find the index of FarmAnimal.type.Value.Equals("Sheep")
						new CodeInstruction(OpCodes.Ldstr, operand: "Sheep"),
						new CodeInstruction(OpCodes.Callvirt, operand: AccessTools.Method(typeof(string), nameof(string.Equals)))
					)
					.Retreat(2)
					.SetOperand(AccessTools.Field(typeof(FarmAnimal), nameof(FarmAnimal.friendshipTowardFarmer)))										// was FarmAnimal.type
					.Advance()
					.SetOperand(AccessTools.Property(typeof(NetFieldBase<Int32, NetInt>), nameof(NetFieldBase<Int32, NetInt>.Value)).GetGetMethod())	// was <string, NetString>
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Nop)	// was Call String.Equals
					)
					.Advance()
					.SetOpCode(OpCodes.Blt_S)				// was Brfalse
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Ldc_I4_0)
					)
					.SetOpCode(OpCodes.Ldc_I4_1)			// was Ldc_I4_0
					.Advance(2)
					.SetOpCode(OpCodes.Ldc_I4_2)			// was Ldc_I4_1
					.Advance()
					.SetOpCode(OpCodes.Div_Un);				// was Sub
			}
			catch (Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Producer produce frequency.\nHelper returned {ex}");
			}
			
			_helper.Backup();

			/// From: if ((!isCoopDweller() && Game1.getFarmer(ownerID).professions.Contains(Farmer.shepherd)) || (isCoopDweller() && Game1.getFarmer(ownerID).professions.Contains(Farmer.butcher)))
			/// To: if (Game1.getFarmer(FarmAnimal.ownerID).professions.Contains(<producer_id>)

			try
			{
				_helper
					.Find(									// find the index of first FarmAnimal.isCoopDweller
						fromCurrentIndex: true,
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.isCoopDweller)))
					)
					.Retreat()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Nop)	// was Ldarg_0
					)
					.Advance()
					.ReplaceWith(
						new CodeInstruction(OpCodes.Nop)	// was Call FarmAnimal.isCoopDweller
					)
					.Advance()
					.GetOperand(out object isNotProducer)
					.ReplaceWith(
						new CodeInstruction(OpCodes.Nop)	// was Brtrue
					)
					.AdvanceUntil(
						new CodeInstruction(OpCodes.Call, operand: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.isCoopDweller)))	// second FarmAnimal.isCoopDweller
					)
					.Insert(
						new CodeInstruction(OpCodes.Br_S, operand: (Label)isNotProducer)														// branch to skip this check if player is not producer
					);
			}
			catch (Exception ex)
			{
				_helper.Restore().Error($"Failed while patching Producer produce quality.\nHelper returned {ex}");
			}

			return _helper.Log("Successful").Flush();
		}
	}
}
