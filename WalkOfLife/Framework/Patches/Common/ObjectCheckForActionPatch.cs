using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ObjectCheckForActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectCheckForActionPatch()
		{
			Original = RequireMethod<SObject>(nameof(SObject.checkForAction));
		}

		#region harmony patches

		/// <summary>Patch to remember object state.</summary>
		[HarmonyPrefix]
		private static bool ObjectCheckForActionPrefix(SObject __instance, ref bool __state)
		{
			__state = __instance.heldObject.Value is not null;
			return true; // run original logic
		}

		/// <summary>Patch to increase Gemologist mineral quality from Crystalarium + increment Ecologist counter for Mushroom Box.</summary>
		[HarmonyPostfix]
		private static void ObjectCheckForActionPostfix(SObject __instance, bool __state, Farmer who)
		{
			if (__instance.heldObject.Value is not null && who.HasProfession("Gemologist") &&
			    (__instance.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer) &&
			    __instance.name == "Crystalarium")
				__instance.heldObject.Value.Quality = Utility.Professions.GetGemologistMineralQuality();
			else if (__state && __instance.heldObject.Value is null && __instance.ParentSheetIndex == 128 &&
			         who.HasProfession("Ecologist"))
				ModEntry.Data.Increment<uint>("ItemsForaged");
		}

		/// <summary>Patch to increment Gemologist counter for gems collected from Crystalarium.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: if (who.professions.Contains(<gemologist_id>) && name.Equals("Crystalarium"))
			///		Data.IncrementField<uint>("MineralsCollected")
			///	Before: switch (name)

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				helper
					.FindLast(
						new CodeInstruction(OpCodes.Ldstr, "coin")
					)
					.Advance(2)
					.Insert(
						// prepare profession check
						new CodeInstruction(OpCodes.Ldarg_1) // arg 1 = Farmer who
					)
					.InsertProfessionCheckForPlayerOnStack(Utility.Professions.IndexOf("Gemologist"),
						dontIncreaseGemologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Call,
							typeof(SObject).PropertyGetter(nameof(SObject.name))),
						new CodeInstruction(OpCodes.Ldstr, "Crystalarium"),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(string).MethodNamed(nameof(string.Equals), new[] {typeof(string)})),
						new CodeInstruction(OpCodes.Brfalse_S, dontIncreaseGemologistCounter),
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "MineralsCollected"),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).MethodNamed(nameof(ModData.Increment), new[] {typeof(string)})
								.MakeGenericMethod(typeof(uint)))
					)
					.AddLabels(dontIncreaseGemologistCounter);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Gemologist counter increment.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}