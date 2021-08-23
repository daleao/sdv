using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GreenSlimeBehaviorAtGameTickPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GreenSlimeBehaviorAtGameTickPatch()
		{
			Original = typeof(GreenSlime).MethodNamed(nameof(GreenSlime.behaviorAtGameTick));
			Prefix = new HarmonyMethod(GetType(), nameof(GreenSlimeBehaviorAtGameTickPrefix));
		}

		#region harmony patches

		/// <summary>Patch to prevent Slimes from jumping on Piper.</summary>
		[HarmonyPrefix]
		private static bool GreenSlimeBehaviorAtGameTickPrefix(GreenSlime __instance, ref int ___readyToJump)
		{
			try
			{
				if (__instance.Player != null && __instance.Player.HasProfession("Piper")) ___readyToJump = -1;
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}

			return true; // run original logic
		}

		#endregion harmony patches
	}
}
