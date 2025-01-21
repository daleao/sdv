namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Core.Framework.Enchantments;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.Integrations;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.Projectiles;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotPerformFirePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SlingshotPerformFirePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and LimitBreak.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            var canDoQuincy = EnchantmentsIntegration.Instance?.IsLoaded == true &&
                              __instance.enchantments
                                  .OfType<BaseSlingshotEnchantment>()
                                  .Any(e => e.GetType().Name.Contains("Quincy")) &&
                              CoreMod.State.AreEnemiesNearby;
            var hasSecondaryAmmo = __instance.attachments.Length > 1 && (__instance.attachments[1] is not null || canDoQuincy);
            if (__instance.attachments[0] is null && !canDoQuincy)
            {
                if (hasSecondaryAmmo && __instance.attachments[1].QualifiedItemId != QIDs.MonsterMusk)
                {
                    __instance.attachments[0] = __instance.attachments[1];
                    __instance.attachments[1] = null;
                }
                else
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                    ___canPlaySound = true;
                    return false; // don't run original logic
                }
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
            {
                return false; // don't run original logic
            }

            // calculate projectile velocity
            Reflector
                .GetUnboundMethodDelegate<Action<Slingshot>>(__instance, "updateAimPos")
                .Invoke(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (xVelocity, yVelocity) = Utility.getVelocityTowardPoint(
                shootOrigin,
                __instance.AdjustForHeight(new Vector2(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.buffs.WeaponSpeedMultiplier));

            // get and spend ammo
            var ammo = (SObject?)__instance.attachments[0]?.getOne() ?? null;
            if (ammo is not null && --__instance.attachments[0].Stack <= 0)
            {
                __instance.attachments[0] = null;
            }

            // calculate damage
            var damageBase = ammo is not null ? __instance.GetAmmoDamage(ammo) : 1;
            var damageMod = __instance.QualifiedItemId switch
            {
                QIDs.MasterSlingshot => 2f,
                QIDs.GalaxySlingshot => 4f,
                _ => 1f,
            };

            // calculate overcharge
            var overcharge = who.HasProfession(Profession.Desperado) ? __instance.GetOvercharge() : 1f;
            if (overcharge > 1f)
            {
                EventManager.Disable<DesperadoOverchargeUpdateTickedEvent>();
            }

            // adjust velocity
            if (Game1.options.useLegacySlingshotFiring)
            {
                xVelocity *= -1f;
                yVelocity *= -1f;
            }

            // add main projectile
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));

            var isMusked = false;
            var isLimitActive = State.LimitBreak is DesperadoBlossom { IsActive: true };
            if (ammo is not null && !isLimitActive && Config.ModKey.IsDown() && hasSecondaryAmmo && __instance.attachments[1] is
                    { QualifiedItemId: QIDs.MonsterMusk } musk)
            {
                var uses = Data.ReadAs(musk, DataKeys.MuskUses, 10);
                if (--uses <= 0)
                {
                    __instance.attachments[1] = null;
                }

                isMusked = true;
                Data.Write(musk, DataKeys.MuskUses, uses.ToString());
            }

            var projectile = ammo is null
                ? EnchantmentsIntegration.Instance!.ModApi!.CreateQuincyProjectile(
                    who,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity,
                    overcharge * overcharge)
                : new ObjectProjectile(
                    ammo,
                    __instance,
                    who,
                    damage,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity,
                    isMusked);

            if (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
            {
                projectile.IgnoreLocationCollision = true;
            }

            location.projectiles.Add(projectile);

            // do Death Blossom
            if (isLimitActive)
            {
                var velocity = new Vector2(xVelocity, yVelocity);
                for (var i = 0; i < 7; i++)
                {
                    velocity = velocity.Rotate(45);
                    if (i == 3)
                    {
                        continue;
                    }

                    damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                    rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                    var petal = ammo is null
                        ? EnchantmentsIntegration.Instance!.ModApi!.CreateQuincyProjectile(
                            who,
                            startingPosition,
                            xVelocity,
                            yVelocity,
                            rotationVelocity,
                            overcharge * overcharge)
                        : new ObjectProjectile(
                            ammo,
                            __instance,
                            who,
                            damage,
                            0f,
                            startingPosition,
                            velocity.X,
                            velocity.Y,
                            rotationVelocity);

                    if (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
                    {
                        petal.IgnoreLocationCollision = true;
                    }

                    who.currentLocation.projectiles.Add(petal);
                }
            }
            else
            // do Prestiged Rascsal double shot
            if (who.HasProfession(Profession.Rascal, true) && Config.ModKey.IsDown() &&
                hasSecondaryAmmo && !isMusked)
            {
                // get and spend ammo
                ammo = (SObject?)__instance.attachments[1]?.getOne() ?? null;
                if (ammo is not null && --__instance.attachments[1].Stack <= 0)
                {
                    __instance.attachments[1] = null;
                }

                // calculate damage
                damageBase = __instance.GetAmmoDamage(ammo);
                damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                var secondProjectile = ammo is null
                    ? EnchantmentsIntegration.Instance!.ModApi!.CreateQuincyProjectile(
                        who,
                        startingPosition,
                        xVelocity,
                        yVelocity,
                        rotationVelocity,
                        overcharge * overcharge)
                    : new ObjectProjectile(
                        ammo,
                        __instance,
                        who,
                        damage,
                        overcharge,
                        startingPosition,
                        xVelocity,
                        yVelocity,
                        rotationVelocity);

                if (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
                {
                    secondProjectile.IgnoreLocationCollision = true;
                }

                Game1.delayedActions.Add(new DelayedAction(225, () => location.projectiles.Add(secondProjectile)));
            }

            foreach (var enchantment in __instance.enchantments.OfType<BaseSlingshotEnchantment>())
            {
                enchantment.OnFire(__instance, projectile, location, who);
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

    /// <summary>Patch to prevent overcharged auto-fire.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SlingshotPerformFirePostfix(Slingshot __instance)
    {
        if (__instance.CanAutoFire())
        {
            __instance.pullStartTime = Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    #endregion harmony patches
}
