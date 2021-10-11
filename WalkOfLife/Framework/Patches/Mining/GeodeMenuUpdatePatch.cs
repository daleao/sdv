using HarmonyLib;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GeodeMenuUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GeodeMenuUpdatePatch()
		{
			Original = typeof(GeodeMenu).MethodNamed(nameof(GeodeMenu.update));
			Transpiler = new HarmonyMethod(GetType(), nameof(GeodeMenuUpdateTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to increment Gemologist counter for geodes cracked at Clint's.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> GeodeMenuUpdateTranspiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator iLGenerator, MethodBase original)
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
							typeof(ModData).MethodNamed(nameof(ModData.IncrementField), new[] { typeof(string) })
								.MakeGenericMethod(typeof(uint)))
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