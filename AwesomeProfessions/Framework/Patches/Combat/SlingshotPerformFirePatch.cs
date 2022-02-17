namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Network;
using StardewValley.Projectiles;
using StardewValley.Tools;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;
using SuperMode;

#endregion using directives

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
        if (!who.HasProfession(Profession.Desperado) ||
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
        if (who.IsLocalPlayer && ModEntry.State.Value.SuperMode is PoacherColdBlood {IsActive: true})
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
        else if (Game1.random.NextDouble() < who.GetDesperadoDoubleStrafeChance())
        {
            if (who.HasProfession(Profession.Desperado, true))
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
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: PerformFireSubroutine(this, damage, velocityTowardPoint, location, who)
        /// Before: if (ammunition.Category == -5) collisionSound = "slimedead";

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_S, $"{typeof(int)} (5)")
                )
                .GetOperand(out var damage)
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloca_S, $"{typeof(Vector2)} (3)")
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
                    // load arguments
                    new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Slingshot instance
                    new CodeInstruction(OpCodes.Ldloca_S, damage),
                    new CodeInstruction(OpCodes.Ldloca_S, velocity),
                    new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = GameLocation location
                    new CodeInstruction(OpCodes.Ldarg_2), // arg 2 = Farmer who
                    new CodeInstruction(OpCodes.Call,
                        typeof(SlingshotPerformFirePatch).MethodNamed(nameof(PerformFireSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while injecting modded Desperado ammunition damage modifier, Temerity gauge and quick shots.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void PerformFireSubroutine(Slingshot slingshot, ref int damage, ref Vector2 velocity, GameLocation location, Farmer who)
    {
        if (!who.IsLocalPlayer || ModEntry.State.Value.SuperMode is not DesperadoTemerity {IsActive: false} desperadoTemerity ||
            !location.IsCombatZone()) return;

        var bulletPower = desperadoTemerity.GetShootingPower();
        velocity *= bulletPower;
        damage = (int) (damage * bulletPower / 5);

        var increment = 12 - 10 * who.health / who.maxHealth;
        if (Game1.currentGameTime.TotalGameTime.TotalSeconds - slingshot.pullStartTime <=
            slingshot.GetRequiredChargeTime() * QUICK_FIRE_HANDICAP_F)
            increment += 6;

        ModEntry.State.Value.SuperMode.ChargeValue += increment * ModEntry.Config.SuperModeGainFactor *
            (double) SuperMode.MaxValue / SuperMode.INITIAL_MAX_VALUE_I;
    }

    #endregion injected subroutines
}