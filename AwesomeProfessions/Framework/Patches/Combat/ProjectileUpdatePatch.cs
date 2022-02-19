namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Projectiles;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using SuperMode;

#endregion using directives

[UsedImplicitly]
internal class ProjectileUpdatePatch : BasePatch
{
    private static readonly FieldInfo _DamagesMonsters = typeof(Projectile).Field("damagesMonsters");

    /// <summary>Construct an instance.</summary>
    internal ProjectileUpdatePatch()
    {
        Original = RequireMethod<Projectile>(nameof(Projectile.update));
    }

    #region harmony patches

    /// <summary>Patch for increased Desperado bullet cross-section.</summary>
    [HarmonyPostfix]
    private static void ProjectileUpdatePostfix(Projectile __instance, ref bool __result, NetPosition ___position,
        NetCharacterRef ___theOneWhoFiredMe, NetFloat ___xVelocity, NetFloat ___yVelocity, GameLocation location)
    {
        // check if is BasicProjectile
        if (__instance is not BasicProjectile projectile) return;

        // check if damages monsters
        var damagesMonsters = ((NetBool) _DamagesMonsters.GetValue(__instance)!).Value;
        if (!damagesMonsters) return;

        // check if firer is has Desperado Super Mode
        var firer = ___theOneWhoFiredMe.Get(Game1.currentLocation) is Farmer farmer ? farmer : Game1.player;
        if (!firer.IsLocalPlayer || ModEntry.State.Value.SuperMode is not DesperadoTemerity desperadoTemerity) return;

        // check for powered bullet
        var bulletPower = desperadoTemerity.GetShootingPower() - 1f;
        if (bulletPower <= 0f) return;

        // check if already collided
        if (__result)
        {
            if (!ModEntry.State.Value.PiercedBullets.Remove(projectile.GetHashCode())) return;

            projectile.damageToFarmer.Value = (int) (projectile.damageToFarmer.Value * 0.6f);
            __result = false;
            return;
        }

        // get collision angle
        var velocity = new Vector2(___xVelocity.Value, ___yVelocity.Value);
        var angle = velocity.AngleWithHorizontal();
        if (angle > 180) angle -= 360;

        // check for extended collision
        var originalHitbox = __instance.getBoundingBox();
        var newHitbox = new Rectangle(originalHitbox.X, originalHitbox.Y, originalHitbox.Width, originalHitbox.Height);
        var isBulletTravelingVertically = Math.Abs(angle) is >= 45 and <= 135;
        if (isBulletTravelingVertically)
        {
            newHitbox.Inflate((int) (originalHitbox.Width * bulletPower), 0);
            if (newHitbox.Width <= originalHitbox.Width) return;
        }
        else
        {
            newHitbox.Inflate(0, (int) (originalHitbox.Height * bulletPower));
            if (newHitbox.Height <= originalHitbox.Height) return;
        }

        if (location.doesPositionCollideWithCharacter(newHitbox) is not Monster monster) return;

        // do deal damage
        int actualDistance, monsterRadius, actualBulletRadius, extendedBulletRadius;
        if (isBulletTravelingVertically)
        {
            actualDistance = Math.Abs(monster.getStandingX() - originalHitbox.Center.X);
            monsterRadius = monster.GetBoundingBox().Width / 2;
            actualBulletRadius = originalHitbox.Width / 2;
            extendedBulletRadius = newHitbox.Width / 2;
        }
        else
        {
            actualDistance = Math.Abs(monster.getStandingY() - originalHitbox.Center.Y);
            monsterRadius = monster.GetBoundingBox().Height / 2;
            actualBulletRadius = originalHitbox.Height / 2;
            extendedBulletRadius = newHitbox.Height / 2;
        }

        var lerpFactor = (actualDistance - (actualBulletRadius + monsterRadius)) /
                         (extendedBulletRadius - actualBulletRadius);
        var multiplier = MathHelper.Lerp(1f, 0f, lerpFactor);
        var damage = (int) (projectile.damageToFarmer.Value * multiplier);
        location.damageMonster(monster.GetBoundingBox(), damage, damage + 1, false, multiplier + bulletPower, 0,
            0f, 1f, true, firer);
    }

    /// <summary>Patch for prestiged Rascal trick shot.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ProjectileUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: BouncedBullets.Add(this.GetHashCode());
        /// After: bouncesLeft.Value--;

        var notTrickShot = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(Projectile).Field("bouncesLeft")),
                    new CodeInstruction(OpCodes.Dup)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetFieldBase<int, NetInt>).PropertySetter("Value"))
                )
                .Advance()
                .AddLabels(notTrickShot)
                .Insert(
                    // check if this is BasicProjectile
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Isinst, typeof(BasicProjectile)),
                    new CodeInstruction(OpCodes.Brfalse_S, notTrickShot),
                    // check if is colliding with monster
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(Projectile).MethodNamed(nameof(Projectile.getBoundingBox))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(GameLocation).MethodNamed(nameof(GameLocation.doesPositionCollideWithCharacter),
                            new[] {typeof(Rectangle), typeof(bool)})),
                    new CodeInstruction(OpCodes.Ldnull),
                    new CodeInstruction(OpCodes.Bgt_Un_S, notTrickShot),
                    // add to bounced bullet set
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).PropertyGetter(nameof(ModEntry.State))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(PerScreen<ModState>).PropertyGetter(nameof(PerScreen<ModState>.Value))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(ModState).PropertyGetter(nameof(ModState.BouncedBullets))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Projectile).MethodNamed(nameof(GetHashCode))),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(HashSet<int>).MethodNamed(nameof(HashSet<int>.Add))),
                    new CodeInstruction(OpCodes.Pop)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Rascal trick shot.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}