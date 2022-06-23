namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;

using DaLion.Common.Harmony;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Xna;
using Extensions;
using Sounds;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : BasePatch
{
    private static readonly FieldInfo _CanPlaySound = typeof(Slingshot).RequireField("canPlaySound")!;
    private static readonly MethodInfo _UpdateAimPos = typeof(Slingshot).RequireMethod("updateAimPos");

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        Prefix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, GameLocation location, Farmer who)
    {
        if (__instance.attachments[0] is null)
        {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
            _CanPlaySound.SetValue(__instance, true);
            return false; // don't run original logic
        }

        var backArmDistance = __instance.GetBackArmDistance(who);
        if (backArmDistance <= 4 || (bool) _CanPlaySound.GetValue(__instance)!)
            return false; // don't run original logic

        // calculate projectile velocity
        _UpdateAimPos.Invoke(__instance, null);
        var mouseX = __instance.aimPos.X;
        var mouseY = __instance.aimPos.Y;
        var shootOrigin = __instance.GetShootOrigin(who);
        var (x, y) = Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
            (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

        // calculate base ammo strength and properties
        var ammo = __instance.attachments[0].getOne();
        if (--__instance.attachments[0].Stack <= 0)
            __instance.attachments[0] = null;

        var damageBase = ammo.ParentSheetIndex switch
        {
            388 => 2, // wood
            390 => 5, // stone
            378 => 10, // copper ore
            380 => 20, // iron ore
            384 => 30, // gold ore
            386 => 50, // iridium ore
            909 => 75, // radioactive ore
            382 => 15, // coal
            441 => 20, // explosive
            _ => 1
        };

        BasicProjectile.onCollisionBehavior collisionBehavior;
        string collisionSound;
        if (ammo.ParentSheetIndex == 441)
        {
            collisionBehavior = BasicProjectile.explodeOnImpact;
            collisionSound = "explosion";
        }
        else
        {
            collisionBehavior = null;
            collisionSound = ammo.Category == -4 ? "slimedead" : "hammer";
            ++ammo.ParentSheetIndex;
        }

        // calculate bonus ammo strength
        var damageMod = __instance.InitialParentTileIndex switch
        {
            33 => 2f,
            34 => 4f,
            _ => 1f
        };

        if (who.HasProfession(Profession.Rascal))
            damageMod *= who.HasProfession(Profession.Rascal, true) ? 1.5f : 1.25f;

        damageMod *= 1f + who.attackIncreaseModifier;

        // check for quick-shot
        var didQuickShot = Game1.currentGameTime.TotalGameTime.TotalSeconds - __instance.pullStartTime <=
                           __instance.GetRequiredChargeTime() + 0.1;
        if (didQuickShot) damageMod *= 0.8f;

        // calculate overcharge
        var overcharge = 1f;
        if (who.HasProfession(Profession.Desperado) && !__instance.CanAutoFire())
            overcharge += __instance.GetDesperadoOvercharge(who);

        // adjust velocity
        if (overcharge > 1f)
        {
            x *= overcharge;
            y *= overcharge;
            who.stopJittering();
            SFX.SinWave?.Stop(AudioStopOptions.Immediate);
        }
        
        if (Game1.options.useLegacySlingshotFiring)
        {
            x *= -1f;
            y *= -1f;
        }

        // calculate bounces
        var bounces = 0;
        if (who.HasProfession(Profession.Rascal) && ModEntry.Config.ModKey.IsDown())
        {
            ++bounces;
            damageMod *= 0.6f;
        }

        // add main projectile
        var startingPosition = shootOrigin - new Vector2(32f, 32f);
        var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod * overcharge;
        var projectile = new BasicProjectile((int) damage, ammo.ParentSheetIndex, bounces, 0,
            (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition,
            collisionSound, "", false, true, location, who, true, collisionBehavior)
        {
            IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null
        };
        location.projectiles.Add(projectile);
        if (overcharge > 1f)
            ModEntry.PlayerState.OverchargedBullets[projectile.GetHashCode()] = overcharge;

        // add auxiliary projectiles
        var velocity = new Vector2(x, y);
        var speed = velocity.Length();
        velocity.Normalize();
        if (who.IsLocalPlayer && ModEntry.PlayerState.RegisteredUltimate is DeathBlossom { IsActive: true })
        {
            // do Death Blossom
            for (var i = 0; i < 7; ++i)
            {
                damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                velocity = velocity.Rotate(45);
                var blossom = new BasicProjectile((int) damage, ammo.ParentSheetIndex, 0, 0,
                    (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                    velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                    true, location, who, true, collisionBehavior)
                {
                    IgnoreLocationCollision =
                        Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
                };

                location.projectiles.Add(blossom);
                ModEntry.PlayerState.BlossomBullets.Add(blossom.GetHashCode());
            }
        }
        else if (overcharge >= 1.5f && who.HasProfession(Profession.Desperado, true) && __instance.attachments[0].Stack >= 2)
        {
            // do spreadshot
            var angle = (int) (MathHelper.Lerp(1f, 0.5f, (overcharge - 1.5f) * 2f) * 15);
            damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            velocity = velocity.Rotate(angle);
            var clockwise = new BasicProjectile((int) damage, ammo.ParentSheetIndex, 0, 0,
                (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                true, location, who, true, collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                          Game1.currentMinigame is not null
            };
            location.projectiles.Add(clockwise);

            damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            velocity = velocity.Rotate(-2 * angle);
            var anticlockwise = new BasicProjectile((int) damage, ammo.ParentSheetIndex, 0, 0,
                (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                true, location, who, true, collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                          Game1.currentMinigame is not null
            };
            location.projectiles.Add(anticlockwise);

            __instance.attachments[0].Stack -= 2;
            if (__instance.attachments[0].Stack <= 0)
                __instance.attachments[0] = null;
        }
        else if (who.HasProfession(Profession.Desperado) && didQuickShot && __instance.attachments[0].Stack >= 1)
        {
            // do double strafe
            damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod * 0.6f;
            var secondary = new BasicProjectile((int) damage, ammo.ParentSheetIndex, 0, 0,
                (float) (Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                velocity.Y * speed, startingPosition, collisionSound, string.Empty, false,
                true, location, who, true, collisionBehavior)
            {
                IgnoreLocationCollision =
                    Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null
            };
            DelayedAction doubleStrafe = new(50, () => { location.projectiles.Add(secondary); });
            Game1.delayedActions.Add(doubleStrafe);

            if (--__instance.attachments[0].Stack <= 0)
                __instance.attachments[0] = null;
        }

        _CanPlaySound.SetValue(__instance, true);
        return false; // don't run original logic
    }

    #endregion harmony patches
}