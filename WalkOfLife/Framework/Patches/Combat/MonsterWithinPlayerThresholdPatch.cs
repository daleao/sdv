using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class MonsterWithinPlayerThresholdPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal MonsterWithinPlayerThresholdPatch()
		{
			Original = typeof(Monster).MethodNamed(nameof(Monster.withinPlayerThreshold), new Type[] { });
			Prefix = new HarmonyMethod(GetType(), nameof(MonsterWithinPlayerThresholdPrefix));
		}

		#region harmony patch

		/// <summary>Patch to make Poacher invisible in super mode.</summary>
		[HarmonyTranspiler]
		private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
		{
			try
			{
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