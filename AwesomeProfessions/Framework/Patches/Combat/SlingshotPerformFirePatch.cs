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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.SuperMode;

namespace TheLion.Stardew.Professions.Framework.Patches.Combat;

[UsedImplicitly]
internal class SlingshotPerformFirePatch : BasePatch
{
    private const float QUICK_FIRE_HANDICAP_F = 1.2f;

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Original = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Patch to perform Desperado Super Mode.</summary>
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
        if (who.IsLocalPlayer && ModEntry.State.Value.SuperMode is {Index: SuperModeIndex.Poacher, IsActive: true})
        {
            // do Death Blossom
            for (var i = 0; i < 7; ++i)
            {
                velocity.Rotate(45);
                var blossom = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision =
                        Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
                };

                location.projectiles.Add(blossom);
                ModEntry.State.Value.AuxiliaryBullets.Add(blossom.GetHashCode());
            }
        }
        else if (Game1.random.NextDouble() < Utility.Professions.GetDesperadoDoubleStrafeChance(who))
        {
            if (who.HasPrestigedProfession("Desperado"))
            {
                // do spreadshot
                velocity.Rotate(15);
                var clockwise = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(clockwise);
                ModEntry.State.Value.AuxiliaryBullets.Add(clockwise.GetHashCode());

                velocity.Rotate(-30);
                var anticlockwise = new BasicProjectile(damage, ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                              Game1.currentMinigame is not null
                };

                location.projectiles.Add(anticlockwise);
                ModEntry.State.Value.AuxiliaryBullets.Add(anticlockwise.GetHashCode());
            }
            else
            {
                // do double strafe
                var secondary = new BasicProjectile((int) (damage.Value * 0.6f), ammunitionIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), 0f - velocity.X * speed,
                    0f - velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision =
                        Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
                };

                DelayedAction doubleStrafe = new(50, () => { location.projectiles.Add(secondary); });
                Game1.delayedActions.Add(doubleStrafe);
                ModEntry.State.Value.AuxiliaryBullets.Add(secondary.GetHashCode());
            }
        }
    }

    /// <summary>Patch to increment Desperado Temerity gauge + add Desperado quick fire projectile velocity bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SlingshotPerformFireTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (who.IsLocalPlayer && location.IsCombatZone() && SuperMode.Index == <desperado_id> && !IsSuperModeActive)
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
                    // restore backed-up labels
                    labels,
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
                    // check if SuperMode is null
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    // check if SuperMode.Index == <desperado_id>
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode.SuperMode).PropertyGetter(nameof(SuperMode.SuperMode.Index))),
                    new CodeInstruction(OpCodes.Ldc_I4_S, (int)SuperModeIndex.Desperado),
                    new CodeInstruction(OpCodes.Bne_Un_S, resumeExecution),
                    // check if SuperMode.IsActive
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode.SuperMode).PropertyGetter(nameof(SuperMode.SuperMode.IsActive))),
                    new CodeInstruction(OpCodes.Brtrue_S, resumeExecution),
                    // v.X *= GetDesperadoBulletPower()
                    new CodeInstruction(OpCodes.Ldloca_S, velocity),
                    new CodeInstruction(OpCodes.Ldflda,
                        typeof(Vector2).Field(nameof(Vector2.X))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldind_R4),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions.GetDesperadoBulletPower))),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stind_R4),
                    // v.Y *= GetDesperadoBulletPower()
                    new CodeInstruction(OpCodes.Ldloca_S, velocity),
                    new CodeInstruction(OpCodes.Ldflda,
                        typeof(Vector2).Field(nameof(Vector2.Y))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldind_R4),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions.GetDesperadoBulletPower))),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Stind_R4),
                    // check for quick shot (i.e. sling shot charge time <= required charge time * handicap)
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
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Slingshot).MethodNamed(nameof(Slingshot.GetRequiredChargeTime))),
                    new CodeInstruction(OpCodes.Ldc_R4, QUICK_FIRE_HANDICAP_F), // <-- handicap
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Bgt_S, notQuickShot),
                    // increment Temerity gauge
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode.SuperMode).PropertyGetter(nameof(SuperMode.SuperMode.Gauge))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.CurrentValue))),
                    new CodeInstruction(OpCodes.Ldc_R8, 10.0), // <-- increment amount
                    new CodeInstruction(OpCodes.Call,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.MaxValue))),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 500.0),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertySetter(nameof(SuperModeGauge.CurrentValue))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Insert(
                    new[] {notQuickShot},
                    // increment Temerity gauge
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.SuperMode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperMode.SuperMode).PropertyGetter(nameof(SuperMode.SuperMode.Gauge))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.CurrentValue))),
                    new CodeInstruction(OpCodes.Ldc_R8, 2.0), // <-- increment amount
                    new CodeInstruction(OpCodes.Call,
                        typeof(SuperModeGauge).PropertyGetter(nameof(SuperModeGauge.MaxValue))),
                    new CodeInstruction(OpCodes.Conv_R8),
                    new CodeInstruction(OpCodes.Ldc_R8, 500.0),
                    new CodeInstruction(OpCodes.Div),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(SuperModeGauge).PropertySetter(nameof(SuperModeGauge.CurrentValue)))
                )
                .AddLabels(resumeExecution); // branch here if is not desperado or can't quick fire
        }
        catch (Exception ex)
        {
            ModEntry.Log(
                $"Failed while injecting modded Desperado ammunition damage modifier, Temerity gauge and quick shots.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}