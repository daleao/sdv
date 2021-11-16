using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class LoomMachineSetInputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal LoomMachineSetInputPatch()
		{
			try
			{
				Original = "LoomMachine".ToType().MethodNamed("SetInput");
			}
			catch
			{
				// ignored
			}
		}

		#region harmony patches

		/// <summary>Patch to apply Artisan effects to automated Loom.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GenericObjectMachineGenericPullRecipeTranspiler(
			IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: GenericPullRecipeSubroutine(this, consumable)
			/// Before: return true;

			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Ldc_I4_1),
						new CodeInstruction(OpCodes.Ret)
					)
					.Insert(
						new CodeInstruction(OpCodes.Ldloc_0),
						new CodeInstruction(OpCodes.Ldloc_1),
						new CodeInstruction(OpCodes.Call,
							typeof(LoomMachineSetInputPatch).MethodNamed(
								nameof(SetInputSubroutine)))
					);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while patching modded Artisan behavior for automated Loom.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static void SetInputSubroutine(SObject machine, object consumable)
		{
			if (!machine.heldObject.Value.IsArtisanGood()) return;

			var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
			if (!who.HasProfession("Artisan")) return;

			var output = machine.heldObject.Value;
			if (consumable.GetType().GetProperty("Sample")?.GetValue(consumable) is SObject input)
				output.Quality = input.Quality;

			if (output.Quality < SObject.bestQuality &&
			    new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
				output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

			machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;
		}

		#endregion private methods
	}
}