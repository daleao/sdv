using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class NPCWithinPlayerThresholdPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal NPCWithinPlayerThresholdPatch()
		{
			Original = typeof(NPC).MethodNamed(nameof(NPC.withinPlayerThreshold), new[] { typeof(int) });
			Prefix = new HarmonyMethod(GetType(), nameof(NPCWithinPlayerThresholdPrefix));
		}

		#region harmony patch

		/// <summary>Patch to make Poacher invisible in super mode.</summary>
		[HarmonyTranspiler]
		private static bool NPCWithinPlayerThresholdPrefix(NPC __instance, ref bool __result)
		{
			try
			{
				if (__instance is not Monster) return true; // run original method

				var foundPlayer = ModEntry.ModHelper.Reflection.GetMethod(__instance, "findPlayer").Invoke<Farmer>();
				if (!foundPlayer.IsLocalPlayer || !ModEntry.IsSuperModeActive ||
					ModEntry.SuperModeIndex != Util.Professions.IndexOf("Poacher")) return true; // run original method

				__result = false;
				return false; // don't run original method
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
				return true; // default to original logic
			}
		}

		#endregion harmony patch
	}
}