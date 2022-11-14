namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Patches;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Enchantments;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Extensions;
using DaLion.Ligo.Modules.Arsenal.Slingshots.Projectiles;
using DaLion.Ligo.Modules.Arsenal.Slingshots.VirtualProperties;
using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Shared.Harmony;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotPerformFirePatcher"/> class.</summary>
    internal SlingshotPerformFirePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.after = new[] { LigoModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("Ligo.Modules.Professions")]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance,
        ref bool ___canPlaySound,
        GameLocation location,
        Farmer who)
    {
        try
        {
            if (__instance.Get_IsOnSpecial())
            {
                return false; // don't run original logic
            }

            Log.D("Did Shoot!");
            var canDoQuincy = __instance.hasEnchantmentOfType<QuincyEnchantment>() && location.HasMonsters();
            if (__instance.attachments[0] is null && !canDoQuincy && !who.IsSteppingOnSnow())
            {
                if (__instance.attachments.Count > 1 && __instance.attachments[1] is not null)
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
            ModEntry.Reflector
                .GetUnboundMethodDelegate<Action<Slingshot>>(__instance, "updateAimPos")
                .Invoke(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (xVelocity, yVelocity) = Utility.getVelocityTowardPoint(
                shootOrigin,
                __instance.AdjustForHeight(new Vector2(mouseX, mouseY)),
                Game1.random.Next(4, 6) + 15);

            // get and spend ammo
            var ammo = __instance.attachments[0]?.getOne();
            var didPreserve = false;
            if (ammo is not null && (!__instance.hasEnchantmentOfType<PreservingEnchantment>() || Game1.random.NextDouble() > 0.5 + (who.DailyLuck / 2) + (who.LuckLevel * 0.01)))
            {
                if (--__instance.attachments[0].Stack <= 0)
                {
                    __instance.attachments[0] = null;
                }
            }
            else
            {
                didPreserve = true;
            }

            var damageBase = ammo?.ParentSheetIndex switch
            {
                SObject.coal => 2,
                SObject.wood => 2,
                SObject.stone => 5,
                SObject.copper => 10,
                SObject.iron => 20,
                SObject.gold => 30,
                SObject.iridium => 50,
                Constants.RadioactiveOreIndex => 80,
                Constants.ExplosiveAmmoIndex => 5,
                Constants.SlimeIndex => who.professions.Contains(Farmer.acrobat) ? 10 : 5,
                null => canDoQuincy ? 5 : 1, // quincy or snowball
                _ => 1, // fish, fruit or vegetable
            };

            // apply slingshot damage modifiers
            var damageMod = __instance.InitialParentTileIndex switch
            {
                Constants.MasterSlingshotIndex => 1.5f,
                Constants.GalaxySlingshotIndex => 2f,
                _ => 1f,
            };

            // calculate overcharge
            var overcharge = ModEntry.Config.EnableProfessions && who.professions.Contains(Farmer.desperado)
                ? __instance.GetOvercharge(who)
                : 1f;

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

            // add main projectile
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
            var index = ammo?.ParentSheetIndex ?? (canDoQuincy
                ? Constants.QuincyProjectileIndex
                : Projectile.snowBall);
            if (ammo is not null && ammo.ParentSheetIndex is not (Constants.ExplosiveAmmoIndex or Constants.SlimeIndex
                    or Constants.RadioactiveOreIndex) && damageBase > 1)
            {
                ++index;
            }

            BasicProjectile projectile = index switch
            {
                Constants.QuincyProjectileIndex => new QuincyProjectile(
                    __instance,
                    who,
                    damage,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity),
                Projectile.snowBall => new SnowballProjectile(
                    who,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity),
                _ => new ObjectProjectile(
                    ammo!,
                    index,
                    __instance,
                    who,
                    damage,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity,
                    !didPreserve),
            };

            if (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
            {
                projectile.IgnoreLocationCollision = true;
            }

            if (__instance.hasEnchantmentOfType<EngorgingEnchantment>())
            {
                projectile.startingScale.Value *= 2f;
            }

            location.projectiles.Add(projectile);
            ___canPlaySound = true;

            // do Death Blossom
            if (who.Get_Ultimate() is DeathBlossom { IsActive: true })
            {
                var velocity = new Vector2(xVelocity, yVelocity);
                for (var i = 0; i < 7; ++i)
                {
                    velocity = velocity.Rotate(45);
                    if (i == 3)
                    {
                        continue;
                    }

                    damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                    rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                    BasicProjectile petal = index switch
                    {
                        Constants.QuincyProjectileIndex => new QuincyProjectile(
                            __instance,
                            who,
                            damage,
                            0f,
                            startingPosition,
                            velocity.X,
                            velocity.Y,
                            rotationVelocity),
                        Projectile.snowBall => new SnowballProjectile(
                            who,
                            0f,
                            startingPosition,
                            velocity.X,
                            velocity.Y,
                            rotationVelocity),
                        _ => new ObjectProjectile(
                            ammo!,
                            index,
                            __instance,
                            who,
                            damage,
                            0f,
                            startingPosition,
                            velocity.X,
                            velocity.Y,
                            rotationVelocity,
                            false),
                    };

                    if (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
                    {
                        petal.IgnoreLocationCollision = true;
                    }

                    who.currentLocation.projectiles.Add(petal);
                }
            }

            foreach (var enchantment in __instance.enchantments.OfType<BaseSlingshotEnchantment>())
            {
                enchantment.OnFire(
                    __instance,
                    projectile,
                    damageBase,
                    damageMod,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    location,
                    who);
            }

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
