using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.Extensions;
using SUtility = StardewValley.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GreenSlimeUpdatePatch : BasePatch
	{
		private const int BASE_INVINCIBILITY_TIMER = 225;

		/// <summary>Construct an instance.</summary>
		internal GreenSlimeUpdatePatch()
		{
			Original = RequireMethod<GreenSlime>(nameof(GreenSlime.update),
				new[] {typeof(GameTime), typeof(GameLocation)});
		}

		#region harmony patches

		/// <summary>Patch for Slimes to damage monsters around Piper.</summary>
		[HarmonyPostfix]
		private static void GreenSlimeUpdatePostfix(GreenSlime __instance, GameLocation location)
		{
			if (!location.DoesAnyPlayerHereHaveProfession("Piper")) return;

			foreach (var npc in __instance.currentLocation.characters.Where(npc =>
				npc.IsMonster && npc is not GreenSlime && npc is not BigSlime))
			{
				var monster = (Monster) npc;
				var monsterBox = monster.GetBoundingBox();
				if (monster.IsInvisible || monster.isInvincible() ||
				    monster.isGlider.Value && __instance.Scale < 1.4f ||
				    !monsterBox.Intersects(__instance.GetBoundingBox()))
					continue;

				if (monster is Bug bug && bug.isArmoredBug.Value // skip armored bugs
				    || monster is LavaCrab && __instance.Sprite.currentFrame % 4 == 0 // skip shelled lava crabs
				    || monster is RockCrab crab && crab.Sprite.currentFrame % 4 == 0 && !ModEntry.ModHelper
					    .Reflection.GetField<NetBool>(crab, "shellGone").GetValue().Value // skip shelled rock crabs
				    || monster is LavaLurk lurk &&
				    lurk.currentState.Value == LavaLurk.State.Submerged // skip submerged lava lurks
				    || monster is Spiker) // skip spikers
					continue;

				var damageToMonster = Math.Max(1,
					__instance.DamageToFarmer + Game1.random.Next(-__instance.DamageToFarmer / 4,
						__instance.DamageToFarmer / 4));

				var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
					? Vector2.Zero
					: SUtility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
				monster.takeDamage(damageToMonster, (int) xTrajectory, (int) yTrajectory, false, 1.0, "slime");
				monster.currentLocation.debris.Add(new(damageToMonster,
					new(monsterBox.Center.X + 16, monsterBox.Center.Y), new(255, 130, 0), 1f,
					monster));
				monster.setInvincibleCountdown(
					(int) (BASE_INVINCIBILITY_TIMER * (ModState.SuperModeIndex == Utility.Professions.IndexOf("Piper")
							? Utility.Professions.GetPiperSlimeAttackSpeedModifier()
							: 1f)
					));
			}
		}

		#endregion harmony patches
	}
}