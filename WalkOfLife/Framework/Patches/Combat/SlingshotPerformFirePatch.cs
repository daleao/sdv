using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class SlingshotPerformFirePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal SlingshotPerformFirePatch()
		{
			Original = typeof(Slingshot).MethodNamed(nameof(Slingshot.PerformFire));
			Postfix = new HarmonyMethod(GetType(), nameof(SlingshotPerformFirePostfix));
			Transpiler = new HarmonyMethod(GetType(), nameof(SlingshotPerformFireTranspiler));
		}

		#region harmony patches

		/// <summary>Patch to perform Desperado super mode.</summary>
		[HarmonyPostfix]
		private static void SlingshotPerformFirePostfix(Slingshot __instance, GameLocation location, Farmer who)
		{
			if (!who.IsLocalPlayer || ModEntry.SuperModeIndex != Util.Professions.IndexOf("Desperado") || location.projectiles.LastOrDefault() is not BasicProjectile mainProjectile) return;

			// get bullet properties
			var damage = mainProjectile.damageToFarmer;
			var xVelocity = ModEntry.ModHelper.Reflection.GetField<NetFloat>(mainProjectile, name: "xVelocity").GetValue().Value;
			var yVelocity = ModEntry.ModHelper.Reflection.GetField<NetFloat>(mainProjectile, name: "yVelocity").GetValue().Value;
			var ammunitionIndex = ModEntry.ModHelper.Reflection.GetField<NetInt>(mainProjectile, name: "currentTileSheetIndex").GetValue().Value;
			var startingPosition = ModEntry.ModHelper.Reflection.GetField<NetPosition>(mainProjectile, name: "position").GetValue().Value;
			var collisionSound = ModEntry.ModHelper.Reflection.GetField<NetString>(mainProjectile, name: "collisionSound").GetValue().Value;
			var collisionBehavior = ModEntry.ModHelper.Reflection.GetField<BasicProjectile.onCollisionBehavior>(mainProjectile, name: "collisionBehavior").GetValue();

			var netVelocity = new Vector2(xVelocity * -1f, yVelocity * -1f);
			var speed = netVelocity.Length();
			netVelocity.Normalize();

			if (ModEntry.IsSuperModeActive)
			{
				// do Death Blossom
				var angle = 0;
				while (angle < 360)
				{
					angle += 45;
					var adjustedVelocity = new Vector2(netVelocity.X, netVelocity.Y).Rotate(angle);

					location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0,
						(float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - adjustedVelocity.X * speed,
						0f - adjustedVelocity.Y * speed, startingPosition, collisionSound, "", explode: false,
						damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
					{
						IgnoreLocationCollision =
							(Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
					});
				}

				//// do Spreadshot
				//var adjustedVelocity = new Vector2(netVelocity.X, netVelocity.Y).Rotate(15);
				//location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - adjustedVelocity.X * speed, 0f - adjustedVelocity.Y * speed, startingPosition, collisionSound, "", explode: false, damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
				//{
				//	IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
				//});

				//adjustedVelocity = new Vector2(netVelocity.X, netVelocity.Y).Rotate(-15);
				//location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - adjustedVelocity.X * speed, 0f - adjustedVelocity.Y * speed, startingPosition, collisionSound, "", explode: false, damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
				//{
				//	IgnoreLocationCollision = (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
				//});
			}
			else if (Game1.random.NextDouble() < Util.Professions.GetDesperadoDoubleStrafeChance())
			{
				// do Double Strafe
				DelayedAction doubleStrafe = new(50, () =>
				{
					location.projectiles.Add(new BasicProjectile(damage, ammunitionIndex, 0, 0,
						(float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - netVelocity.X * speed,
						0f - netVelocity.Y * speed, startingPosition, collisionSound, "", explode: false,
						damagesMonsters: true, location, who, spriteFromObjectSheet: true, collisionBehavior)
					{
						IgnoreLocationCollision =
							(Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
					});
				});
				Game1.delayedActions.Add(doubleStrafe);
			}
		}

		/// <summary>Patch to increase Desperado ammunition damage modifier + increment Desperado Cold Blood counter + add Desperado quick fire projectile velocity bonus.</summary>
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
		{
			Helper.Attach(original, instructions);

			/// Injected: damage *= 1.5f
			///			  if (who.IsLocalPlayer && SuperModeIndex == <desperado_id> && !IsSuperModeActive)
			///				  if (Game1.currentTime.TotalGameTime.TotalSeconds - this.pullStartTime <= GetDesperadoChargeTime()* breathingRoom) { SuperModeCounter += 10; v *= 1.5f; }
			///				  else { SuperModeCounter += 2 }
			/// Before: if (ammunition.Category == -5) collisionSound = "slimedead";

			var notQuickShot = iLGenerator.DefineLabel();
			var resumeExecution = iLGenerator.DefineLabel();
			var chargeTime = iLGenerator.DeclareLocal(typeof(TimeSpan));
			try
			{
				Helper
					.FindFirst(
						new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
					)
					.GetOperand(out var damage) // copy reference to local 5 = damage
					.FindNext(
						new CodeInstruction(OpCodes.Ldloca_S)
					)
					.GetOperand(out var velocity) // copy reference to local 3 = v (velocity)
					.FindFirst( // find index of ammunition.Category
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Item).PropertyGetter(nameof(Item.Category)))
					)
					.Retreat()
					.GetLabels(out var labels) // backup branch labels
					.StripLabels() // remove labels from here
					.Insert(
						// multiply ammunition damage by 1.5f
						new CodeInstruction(OpCodes.Ldloc_S, damage),
						new CodeInstruction(OpCodes.Ldc_R4, 1.5f),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Conv_I4),
						new CodeInstruction(OpCodes.Stloc_S, damage),
						// check if who.IsLocalPlayer)
						new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = Farmer who
						new CodeInstruction(OpCodes.Callvirt,
							typeof(Farmer).PropertyGetter(nameof(Farmer.IsLocalPlayer))),
						new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
						// check if SuperModeIndex == <desperado_id>
						new CodeInstruction(OpCodes.Call,
							typeof(ModEntry).PropertyGetter(nameof(ModEntry.SuperModeIndex))),
						new CodeInstruction(OpCodes.Ldc_I4_S, Util.Professions.IndexOf("Desperado")),
						new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
						// check if IsSuperModeActive = true
						new CodeInstruction(OpCodes.Call, typeof(ModEntry).PropertyGetter(nameof(ModEntry.IsSuperModeActive))),
						new CodeInstruction(OpCodes.Brtrue_S, resumeExecution),
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
							typeof(Util.Professions).MethodNamed(nameof(Util.Professions.GetDesperadoChargeTime))),
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
						// v.X *= GetDesperadoProjectileTravelSpeedModifier()
						new CodeInstruction(OpCodes.Ldloca_S, velocity),
						new CodeInstruction(OpCodes.Ldflda,
							typeof(Vector2).Field(nameof(Vector2.X))),
						new CodeInstruction(OpCodes.Dup),
						new CodeInstruction(OpCodes.Ldind_R4),
						new CodeInstruction(OpCodes.Ldc_R4, 1.8f),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stind_R4),
						// v.Y *= GetDesperadoProjectileTravelSpeedModifier()
						new CodeInstruction(OpCodes.Ldloca_S, velocity),
						new CodeInstruction(OpCodes.Ldflda,
							typeof(Vector2).Field(nameof(Vector2.Y))),
						new CodeInstruction(OpCodes.Dup),
						new CodeInstruction(OpCodes.Ldind_R4),
						new CodeInstruction(OpCodes.Ldc_R4, 1.8f),
						new CodeInstruction(OpCodes.Mul),
						new CodeInstruction(OpCodes.Stind_R4),
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
				Helper.Error($"Failed while injecting modded Desperado ammunition damage modifier, Cold Blood counter and quick shots.\nHelper returned {ex}");
				return null;
			}

			return Helper.Flush();
		}

		#endregion harmony patches
	}
}