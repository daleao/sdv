using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class SlingshotPerformFirePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotPerformFirePatch()
		{
			Original = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
			Postfix = new(AccessTools.Method(GetType(), nameof(SlingshotPerformFirePostfix)));
			Transpiler = new(AccessTools.Method(GetType(), nameof(SlingshotPerformFireTranspiler)));
		}

		#region harmony patches

		/// <summary>Patch to perform Desperado super mode.</summary>
		[HarmonyPostfix]
		private static void SlingshotPerformFirePostfix(GameLocation location, Farmer who)
		{
			if (!who.HasProfession("Desperado") ||
			    location.projectiles.LastOrDefault() is not BasicProjectile mainProjectile) return;

			// get bullet properties
			var damage = mainProjectile.damageToFarmer;
			var xVelocity = ModEntry.ModHelper.Reflection.GetField<NetFloat>(mainProjectile, "xVelocity").GetValue()
				.Value;
			var yVelocity = ModEntry.ModHelper.Reflection.GetField<NetFloat>(mainProjectile, "yVelocity").GetValue()
				.Value;
			var ammunitionIndex = ModEntry.ModHelper.Reflection
				.GetField<NetInt>(mainProjectile, "currentTileSheetIndex").GetValue().Value;
			var startingPosition = ModEntry.ModHelper.Reflection.GetField<NetPosition>(mainProjectile, "position")
				.GetValue().Value;
			var collisionSound = ModEntry.ModHelper.Reflection.GetField<NetString>(mainProjectile, "collisionSound")
				.GetValue().Value;
			var collisionBehavior = ModEntry.ModHelper.Reflection
				.GetField<BasicProjectile.onCollisionBehavior>(mainProjectile, "collisionBehavior").GetValue();

			var velocity = new Vector2(xVelocity * -1f, yVelocity * -1f);
			var speed = velocity.Length();
			velocity.Normalize();
			if (who.IsLocalPlayer && ModEntry.IsSuperModeActive &&
			    ModEntry.SuperModeIndex == Utility.Professions.IndexOf("Desperado"))
			{
				// do Death Blossom
				;
				for (var i = 0; i < 7; ++i)
				{
					velocity.Rotate(45);
					location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0,
						(float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
						0f - velocity.Y * speed, startingPosition, collisionSound, "", false,
						true, location, who, true, collisionBehavior)
					{
						IgnoreLocationCollision =
							Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
					});
				}

				//// do Spreadshot
				//var adjustedVelocity = new Vector2(netVelocity.X, netVelocity.Y).Rotate(15);
				//location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - adjustedVelocity.X * speed, 0f - adjustedVelocity.Y * speed, startingPosition, collisionSound, "", explode: false, damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
				//{
				//	IgnoreLocationCollision = (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
				//});

				//adjustedVelocity = new Vector2(netVelocity.X, netVelocity.Y).Rotate(-15);
				//location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - adjustedVelocity.X * speed, 0f - adjustedVelocity.Y * speed, startingPosition, collisionSound, "", explode: false, damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
				//{
				//	IgnoreLocationCollision = (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
				//});
			}
			else if (Game1.random.NextDouble() < Utility.Professions.GetDesperadoDoubleStrafeChance(who))
			{
				// do Double Strafe
				DelayedAction doubleStrafe = new(50, () =>
				{
					location.projectiles.Add(new BasicProjectile((int) (damage.Value * 0.6f), ammunitionIndex, 0, 0,
						(float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
						0f - velocity.Y * speed, startingPosition, collisionSound, "", false,
						true, location, who, true, collisionBehavior)
					{
						IgnoreLocationCollision =
							Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
					});
				});
				Game1.delayedActions.Add(doubleStrafe);
			}
		}

		/// <summary>Patch to increment Desperado Cold Blood counter + add Desperado quick fire projectile velocity bonus.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(
			IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			var helper = new ILHelper(original, instructions);

			/// Injected: if (who.IsLocalPlayer && location.IsCombatZone() && SuperModeIndex == <desperado_id> && !IsSuperModeActive)
			///				v *= GetDesperadoBulletPower();
			///				if (Game1.currentTime.TotalGameTime.TotalSeconds - this.pullStartTime <= GetDesperadoChargeTime()* breathingRoom) { SuperModeCounter += 10; }
			///				else { SuperModeCounter += 2 }
			/// Before: if (ammunition.Category == -5) collisionSound = "slimedead";

			var notQuickShot = iLGenerator.DefineLabel();
			var resumeExecution = iLGenerator.DefineLabel();
			var chargeTime = iLGenerator.DeclareLocal(typeof(TimeSpan));
			try
			{
				helper
					.FindFirst(
						new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
					)
					.FindNext(
						new CodeInstruction(OpCodes.Ldloca_S)
					)
					.GetOperand(out var velocity) // copy reference to local 3 = v (velocity)
					.FindFirst( // find index of ammunition.Category
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Item).PropertyGetter(nameof(Item.Category)))
					)
					.Retreat()
					.StripLabels(out var labels) // backup and remove branch labels
					.Insert(
						// check if who.IsLocalPlayer)
						new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = Farmer who
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
						new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
						// check if location.IsCombatZone
						new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = GameLocation location
						new CodeInstruction(OpCodes.Call,
							typeof(GameLocationExtensions).MethodNamed(nameof(GameLocationExtensions.IsCombatZone))),
						new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
						// check if SuperModeIndex == <desperado_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Utility.Professions.IndexOf("Desperado")),
						new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
						// check if IsSuperModeActive = true
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brtrue_S, resumeExecution),
						// v.X *= GetDesperadoBulletPower()
						new CodeInstruction(OpCodes.Ldloca_S, velocity),
						new CodeInstruction(OpCodes.Ldflda,
							typeof(Vector2).Field(nameof(Vector2.X))),
						new CodeInstruction(OpCodes.Dup),
						new CodeInstruction(OpCodes.Ldind_R4),
						new CodeInstruction(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions
								.GetDesperadoBulletPower))),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stind_R4),
						// v.Y *= GetDesperadoBulletPower()
						new CodeInstruction(OpCodes.Ldloca_S, velocity),
						new CodeInstruction(OpCodes.Ldflda,
							typeof(Vector2).Field(nameof(Vector2.Y))),
						new CodeInstruction(OpCodes.Dup),
						new CodeInstruction(OpCodes.Ldind_R4),
						new CodeInstruction(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions
								.GetDesperadoBulletPower))),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stind_R4),
						// check for quick shot (i.e. sling shot charge time <= required charge time * breathing room)
						new CodeInstruction(OpCodes.Ldsfld,
							typeof(Game1).Field(nameof(Game1.currentGameTime))),
						new CodeInstruction(OpCodes.Callvirt,
							typeof(GameTime).PropertyGetter(nameof(GameTime.TotalGameTime))),
						new CodeInstruction(OpCodes.Stloc_S, chargeTime),
						new CodeInstruction(OpCodes.Ldloca_S, chargeTime),
						new CodeInstruction(OpCodes.Call,
							typeof(TimeSpan).PropertyGetter(nameof(TimeSpan.TotalSeconds))),
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld,
							typeof(Slingshot).Field(nameof(Slingshot.pullStartTime))),
						new CodeInstruction(OpCodes.Sub),
						new CodeInstruction(OpCodes.Call,
							typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions
								.GetDesperadoChargeTime))),
						new CodeInstruction(OpCodes.Ldc_R4, 1.2f), // <-- breathing room
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Bgt_S, notQuickShot),
						// increment Cold Blood counter
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeCounter))),
						new CodeInstruction(OpCodes.Ldc_I4_S, 10), // <-- increment amount
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertySetter(nameof(ModEntry.SuperModeCounter))),
						new CodeInstruction(OpCodes.Br_S, resumeExecution)
					)
					.Insert(
						// increment Cold Blood counter
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeCounter))),
						new CodeInstruction(OpCodes.Ldc_I4_S, 2), // <-- increment amount
						new CodeInstruction(OpCodes.Add),
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertySetter(nameof(ModEntry.SuperModeCounter)))
					)
					.AddLabels(resumeExecution) // branch here if is not desperado or can't quick fire
					.Return()
					.AddLabels(notQuickShot)
					.Return()
					.AddLabels(labels); // restore backed-up labels to inserted checks
			}
			catch (Exception ex)
			{
				ModEntry.Log(
					$"Failed while injecting modded Desperado ammunition damage modifier, Cold Blood counter and quick shots.\nHelper returned {ex}",
					LogLevel.Error);
				return null;
			}

			return helper.Flush();
		}

		#endregion harmony patches
	}
}