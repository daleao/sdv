namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using System.Reflection;
using DaLion.Redux.Arsenal.Slingshots.Enchantments;
using DaLion.Redux.Core.Extensions;
using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Xna;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotPerformFirePatch"/> class.</summary>
    internal SlingshotPerformFirePatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.after = new[] { ReduxModule.Arsenal.Name };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.Redux.Arsenal")]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            var ammoIndex = ModEntry.State.Professions.UsingPrimaryAmmo ? 0 : 1;
            var hasQuincyEnchantment = __instance.hasEnchantmentOfType<QuincyEnchantment>();
            var hasPreservingEnchantment = __instance.hasEnchantmentOfType<PreservingEnchantment>();
            if (__instance.attachments[ammoIndex] is null && !hasQuincyEnchantment)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
            {
                return false; // don't run original logic
            }

            // calculate projectile velocity
            ModEntry.Reflector
                .GetUnboundMethodDelegate<Action<Slingshot>>(__instance, "updateAimPos")
                .Invoke(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (xVelocity, yVelocity) = Utility.getVelocityTowardPoint(
                shootOrigin,
                __instance.AdjustForHeight(new Vector2(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            // get and spend ammo
            var ammo = __instance.attachments[ammoIndex]?.getOne();
            var didPreserve = hasPreservingEnchantment &&
                              Game1.random.NextDouble() < 0.5 + (who.DailyLuck / 2) + (who.LuckLevel * 0.01);
            if (ammo is not null && !didPreserve && --__instance.attachments[ammoIndex].Stack <= 0)
            {
                __instance.attachments[ammoIndex] = null;
            }

            var damageBase = ammo?.ParentSheetIndex switch
            {
                SObject.wood => 2,
                SObject.stone => 5,
                SObject.copper => 10,
                SObject.iron => 20,
                SObject.gold => 30,
                SObject.iridium => 50,
                SObject.coal => 15,
                Constants.RadioactiveOreIndex => 75,
                Constants.ExplosiveAmmoIndex => 20,
                Constants.SlimeIndex => who.HasProfession(Profession.Piper, true) ? 10 : 5,
                null => 5, // quincy
                _ => 1, // fish, fruit or vegetable
            };

            // set collision properties
            BasicProjectile.onCollisionBehavior? collisionBehavior = null;
            string collisionSound;
            switch (ammo?.ParentSheetIndex)
            {
                case 441:
                    collisionBehavior = BasicProjectile.explodeOnImpact;
                    collisionSound = "explosion";
                    break;
                case 909 or 766:
                    collisionSound = ammo.ParentSheetIndex == 766 ? "slimedead" : "hammer";
                    break;
                case null:
                    collisionSound = "debuffHit";
                    break;
                default:
                    collisionSound = ammo.IsSquishyAmmo() ? "slimedead" : "hammer";
                    if (damageBase > 1)
                    {
                        ++ammoIndex;
                    }

                    break;
            }

            // apply slingshot damage modifiers
            var damageMod = __instance.InitialParentTileIndex switch
            {
                Constants.MasterSlingshotIndex => 2f,
                Constants.GalaxySlingshotIndex => 4f,
                _ => 1f,
            };

            // calculate overcharge
            var overcharge = 1f;
            if (who.HasProfession(Profession.Desperado) && !__instance.CanAutoFire())
            {
                overcharge += __instance.GetOvercharge(who);
            }

            // adjust velocity
            if (overcharge > 1f)
            {
                xVelocity *= overcharge;
                yVelocity *= overcharge;
                ModEntry.Events.Disable<DesperadoUpdateTickedEvent>();
            }

            if (Game1.options.useLegacySlingshotFiring)
            {
                xVelocity *= -1f;
                yVelocity *= -1f;
            }

            // calculate bounces
            var bounces = 0;
            if (who.HasProfession(Profession.Rascal) && ammo?.IsSquishyAmmo() == false &&
                ModEntry.Config.Professions.ModKey.IsDown())
            {
                ++bounces;
            }

            // add main projectile
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod * overcharge;
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var projectile = new ReduxProjectile(
                ammo,
                __instance,
                who,
                location,
                damage,
                overcharge,
                bounces,
                startingPosition,
                xVelocity,
                yVelocity,
                rotationVelocity,
                collisionSound,
                collisionBehavior,
                !didPreserve)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null,
            };

            if (ammo is null)
            {
                projectile.startingScale.Value *= overcharge * overcharge;
            }

            location.projectiles.Add(projectile);

            // add auxiliary projectiles
            var velocity = new Vector2(xVelocity, yVelocity);
            var speed = velocity.Length();
            velocity.Normalize();
            if (who.IsLocalPlayer && who.Get_Ultimate() is DeathBlossom { IsActive: true })
            {
                // do Death Blossom
                for (var i = 0; i < 7; ++i)
                {
                    velocity = velocity.Rotate(45);
                    if (i == 3)
                    {
                        continue;
                    }

                    damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                    rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                    var adjustedVelocity = new Vector2(velocity.X, velocity.Y) * speed;
                    var petal = new ReduxProjectile(
                        ammo,
                        __instance,
                        who,
                        location,
                        damage,
                        overcharge,
                        bounces,
                        startingPosition,
                        adjustedVelocity.X,
                        adjustedVelocity.Y,
                        rotationVelocity,
                        collisionSound,
                        collisionBehavior,
                        false)
                    {
                        IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                                  Game1.currentMinigame is not null,
                    };

                    location.projectiles.Add(petal);
                }
            }

            ___canPlaySound = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
