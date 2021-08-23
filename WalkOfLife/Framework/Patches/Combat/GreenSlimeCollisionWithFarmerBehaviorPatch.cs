using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Events;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
	{
		private static readonly SlimeContactTimerCountdownUpdateTickedEvent PiperUpdateTickedEvent = new();

		/// <summary>Construct an instance.</summary>
		internal GreenSlimeCollisionWithFarmerBehaviorPatch()
		{
			Original = typeof(GreenSlime).MethodNamed(nameof(GreenSlime.collisionWithFarmerBehavior));
			Postfix = new HarmonyMethod(GetType(), nameof(GreenSlimeCollisionWithFarmerBehaviorPostfix));
		}

		#region harmony patches

		/// <summary>Patch to increment Piper Eubstance counter and heal on contact with slime.</summary>
		[HarmonyPostfix]
		private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
		{
			try
			{
				var who = __instance.Player;
				if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != Util.Professions.IndexOf("Piper") || ModEntry.SlimeContactTimer > 0) return;

				who.health = Math.Min(who.health + 1, who.maxHealth);
				__instance.currentLocation.debris.Add(new Debris(1, new Vector2(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
				ModEntry.SuperModeCounter += 2;
				ModEntry.SlimeContactTimer = 60;
				ModEntry.Subscriber.Subscribe(PiperUpdateTickedEvent);
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}
