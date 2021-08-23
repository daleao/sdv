using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Linq;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class GreenSlimeUpdatePatch : BasePatch
	{
		private const int BASE_INVINCIBILITY_TIMER = 225;

		/// <summary>Construct an instance.</summary>
		internal GreenSlimeUpdatePatch()
		{
			Original = typeof(GreenSlime).MethodNamed(nameof(GreenSlime.update), new[] { typeof(GameTime), typeof(GameLocation) });
			Postfix = new HarmonyMethod(GetType(), nameof(GreenSlimeUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch for Slimes to damage monsters around Piper.</summary>
		[HarmonyPostfix]
		private static void GreenSlimeUpdatePostfix(GreenSlime __instance, GameLocation location)
		{
			try
			{
				if (!location.DoesAnyPlayerHereHaveProfession("Piper")) return;

				foreach (var npc in __instance.currentLocation.characters.Where(npc => npc.IsMonster && npc is not GreenSlime))
				{
					var monster = (Monster)npc;
					var monsterBox = monster.GetBoundingBox();
					if (monster.IsInvisible || monster.isInvincible() || monster.isGlider.Value || !monsterBox.Intersects(__instance.GetBoundingBox()))
						continue;

					if (monster is Bug bug && bug.isArmoredBug.Value // skip armored bugs
						|| monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0 // skip shelled lava crabs
						|| monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 && !ModEntry.Reflection.GetField<NetBool>(crab, name: "shellGone").GetValue().Value // skip shelled rock crabs
						|| monster is LavaLurk lurk && lurk.currentState.Value == LavaLurk.State.Submerged // skip submerged lava lurks
						|| monster is Spiker) // skip spikers
						continue;

					var damageToMonster = Math.Max(1, __instance.DamageToFarmer + Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4));
					var trajectory = SUtility.getAwayFromPositionTrajectory(monsterBox, __instance.Position) / 2f;
					monster.takeDamage(damageToMonster, (int)trajectory.X, (int)trajectory.Y, isBomb: false, 1.0, hitSound: "slime");
					monster.setInvincibleCountdown((int)(BASE_INVINCIBILITY_TIMER * Util.Professions.GetCooldownOrChargeTimeReduction()));
					monster.currentLocation.debris.Add(new Debris(damageToMonster, new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y), new Color(255, 130, 0), 1f, monster));
				}
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}