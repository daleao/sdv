using HarmonyLib;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ProducerRuleControllerPrepareOutputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ProducerRuleControllerPrepareOutputPatch()
		{
			Original = AccessTools.Method("ProducerFrameworkMod.Controllers.ProducerRuleController:PrepareOutput");
			//Transpiler = new HarmonyMethod(GetType(), nameof(ProducerRuleControllerPrepareOutputTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to apply modded Artisan and Gemologist quality rules to PFM machines.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ProducerRuleControllerPrepareOutputTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: output = IncreaseQualityIfNecessary(output, input, producer, keepInputQuality, who)
			/// After: output = OutputConfigController.CreateOutput( ... )
			/// Before: producer.heldObject.set_Value(output)

			var pfmAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName.StartsWith("ProducerFrameworkMod,"));
			var keepInputQuality = AccessTools.GetDeclaredFields(pfmAssembly.GetType("ProducerFrameworkMod.ContentPack.OutputConfig")).Find(f => f.Name == "KeepInputQuality");
			try
			{
				Helper
					.FindFirst( // find instruction to load the producer instance
						new CodeInstruction(OpCodes.Ldloc_0),
						new CodeInstruction(OpCodes.Ldfld)
					)
					.ToBuffer(2) // copy those instructions
					.FindFirst( // find local 5 = outputConfig
						new CodeInstruction(OpCodes.Ldloc_S, "ProducerFrameworkMod.ContentPack.OutputConfig (5)")
					)
					.GetOperand(out var local5) // copy local variable reference
					.FindFirst( // find local 8 = SObject input
						new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(SObject)} (8)")
					)
					.GetOperand(out var local8) // copy local variable reference
					.FindFirst( // find instruction to set the producer held object value
						new CodeInstruction(OpCodes.Callvirt, typeof(NetFieldBase<SObject, NetRef<SObject>>).MethodNamed("set_Value", new[] { typeof(SObject) }))
					) // after this the output is already on the stack
					.Insert( // load the input next
						new CodeInstruction(OpCodes.Ldloc_S, local8)
					)
					.InsertBuffer() // paste instructions to load the producer instance
					.Insert(
						// load objectConfig.KeepInputQuality
						new CodeInstruction(OpCodes.Ldloc_S, local5),
						new CodeInstruction(OpCodes.Ldfld, keepInputQuality),
						// load Farmer who
						new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = Farmer who
															  // call custom logic
						new CodeInstruction(OpCodes.Call, typeof(ProducerRuleControllerPrepareOutputPatch).MethodNamed(nameof(PrepareOutputSubroutine)))
					);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while patching PFM for Artisan and Gemologist machine output quality.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches

		#region private methods

		private static SObject PrepareOutputSubroutine(SObject output, SObject input, SObject producer, bool keepInputQuality, Farmer who)
		{
			if (producer.IsArtisanMachine() && input.IsArtisanGood() && who.HasProfession("Artisan"))
			{
				if (!keepInputQuality)
					output.Quality = input?.Quality ?? 0;

				if (output.Quality < SObject.bestQuality && new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
					output.Quality += output.Quality == SObject.medQuality ? 2 : 1;
			}
			else if (producer.name == "Geode Crusher" && (input.IsForagedMineral() || input.IsGemOrMineral()) && who.HasProfession("Gemologist"))
			{
				output.Quality = Util.Professions.GetGemologistMineralQuality();
				if (who.IsLocalPlayer) ModEntry.Data.IncrementField<uint>("MineralsCollected");
			}

			return output;
		}

		#endregion private methods
	}
}