using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectPerformObjectDropInActionPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectPerformObjectDropInActionPatch()
		{
			Original = typeof(SObject).MethodNamed(nameof(SObject.performObjectDropInAction));
			Prefix = new(GetType(), nameof(ObjectPerformObjectDropInActionPrefix));
			Postfix = new(GetType(), nameof(ObjectPerformObjectDropInActionPostfix));
			Transpiler = new(GetType(), nameof(ObjectPerformObjectDropInActionTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to remember initial machine state.</summary>
		[HarmonyPrefix]
		// ReSharper disable once RedundantAssignment
		private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
		{
			__state = __instance.heldObject.Value !=
			          null; // remember whether this machine was already holding an object
			return true; // run original logic
		}

		/// <summary>
		///     Patch to increase Gemologist mineral quality from Geode Crusher and Crystalarium + speed up Artisan production
		///     speed + integrate Quality Artisan Products.
		/// </summary>
		[HarmonyPostfix]
		private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
			bool probe, Farmer who)
		{
			try
			{
				// if there was an object inside before running the original method, or if the machine is still empty after running the original method, or if the machine doesn't belong to this player, then do nothing
				if (__state || __instance.heldObject.Value == null ||
				    Context.IsMultiplayer && __instance.owner.Value != who.UniqueMultiplayerID || probe) return;

				if (__instance.name.AnyOf("Crystalarium", "Geode Crusher") && who.HasProfession("Gemologist") &&
				    (__instance.heldObject.Value.IsForagedMineral() || __instance.heldObject.Value.IsGemOrMineral()))
				{
					__instance.heldObject.Value.Quality = Util.Professions.GetGemologistMineralQuality();
				}
				else if (__instance.IsArtisanMachine() && dropInItem is SObject dropIn)
				{
					// mead cares about input honey flower type
					if (__instance.name == "Keg" && dropIn.ParentSheetIndex == 340 &&
					    dropIn.preservedParentSheetIndex.Value > 0)
					{
						__instance.heldObject.Value.preservedParentSheetIndex.Value =
							dropIn.preservedParentSheetIndex.Value;
						__instance.heldObject.Value.Price = dropIn.Price * 2;
					}
					// large milk/eggs give double output
					else if (__instance.name.AnyOf("Mayonnaise Machine", "Cheese Press") &&
					         dropIn.name.Contains("Large"))
					{
						__instance.heldObject.Value.Stack = 2;
					}

					if (who.HasProfession("Artisan"))
					{
						__instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;
						__instance.heldObject.Value.Quality = dropIn.Quality;
						if (dropIn.Quality < SObject.bestQuality &&
						    new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
							__instance.heldObject.Value.Quality +=
								dropIn.Quality == SObject.medQuality ? 2 : dropIn.Quality + 1;
					}
					else
					{
						__instance.heldObject.Value.Quality = SObject.lowQuality;
					}
				}
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
			}
		}

		/// <summary>Patch to increment Gemologist counter for geodes cracked by Geode Crusher.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ObjectPerformObjectDropInActionTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: if (Game1.player.professions.Contains(<gemologist_id>))
			///		Data.IncrementField<uint>("MineralsCollected")
			///	After: Game1.stats.GeodesCracked++;

			var dontIncreaseGemologistCounter = iLGenerator.DefineLabel();
			try
			{
				Helper
					.FindNext(
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Stats).PropertySetter(nameof(Stats.GeodesCracked)))
					)
					.Advance()
					.InsertProfessionCheckForLocalPlayer(Util.Professions.IndexOf("Gemologist"),
						dontIncreaseGemologistCounter)
					.Insert(
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.Data))),
						new CodeInstruction(OpCodes.Ldstr, "MineralsCollected"),
						new CodeInstruction(OpCodes.Call,
							typeof(ModData).MethodNamed(nameof(ModData.IncrementField), new[] {typeof(string)})
								.MakeGenericMethod(typeof(uint)))
					)
					.AddLabels(dontIncreaseGemologistCounter);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed while adding Gemologist counter increment.\nHelper returned {ex}", LogLevel.Error);
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}