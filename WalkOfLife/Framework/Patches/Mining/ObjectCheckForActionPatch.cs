using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectCheckForActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectCheckForActionPatch()
		{
			Original = typeof(SObject).MethodNamed(nameof(SObject.checkForAction));
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectCheckForActionPostfix));
			Transpiler = new HarmonyMethod(GetType(), nameof(ObjectCheckForActionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to increase Gemologist mineral quality from Crystalarium.</summary>
		[HarmonyPostfix]
		private static void ObjectCheckForActionPostfix(SObject __instance, Farmer who)
		{
			try
			{
				if (__instance.heldObject.Value == null || !who.HasProfession("Gemologist") || !(__instance.owner.Value == who.UniqueMultiplayerID || !Game1.IsMultiplayer))
					return;

				if (__instance.name.Equals("Crystalarium")) __instance.heldObject.Value.Quality = Util.Professions.GetGemologistMineralQuality();
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch to increment Gemologist counter for gems collected from Crystalarium.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: if (who.professions.Contains(<gemologist_id>) && name.Equals("Crystalarium"))
			///		Data.IncrementField<uint>("MineralsCollected")
			///	Before: switch (name) 

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindLast(
						new CodeInstruction(OpCodes.Ldstr, "coin")
					)
					.Advance(2)
					.Insert(
						// prepare profession check
						new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Util.Professions.IndexOf("Gemologist"),
						dontIncreaseGemologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call,
							typeof(SObject).PropertyGetter(nameof(SObject.name))),
						new CodeInstruction(OpCodes.Ldstr, "Crystalarium"),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(string).MethodNamed(nameof(string.Equals), new[] { typeof(string) })),
						new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "MineralsCollected"),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).MethodNamed(nameof(ModData.IncrementField), new[] { typeof(string) }).MakeGenericMethod(typeof(uint)))
					)
					.AddLabels(dontIncreaseGemologistCounter);
			}
			catch (Exception ex)
			{
				Helper.Error($"Failed while adding Gemologist counter increment.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}