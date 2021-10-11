using System;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Monsters;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class DustSpiritBehaviorAtGameTickPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal DustSpiritBehaviorAtGameTickPatch()
		{
			Original = typeof(DustSpirit).MethodNamed(nameof(DustSpirit.behaviorAtGameTick));
			Postfix = new HarmonyMethod(GetType(), nameof(DustSpiritBehaviorAtGameTickPostfix));
		}

		#region harmony patches

		/// <summary>Patch to hide Poacher from Dust Spirits during super mode.</summary>
		[HarmonyPostfix]
		private static void DustSpiritBehaviorAtGameTickPostfix(DustSpirit __instance, ref bool ___seenFarmer)
		{
			try
			{
				if (!__instance.Player.IsLocalPlayer || !ModEntry.IsSuperModeActive ||
				    ModEntry.SuperModeIndex != Util.Professions.IndexOf("Poacher")) return;
				___seenFarmer = false;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}