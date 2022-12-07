namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Projectiles;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
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
    internal SlingshotPerformFirePatcher()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        this.Prefix!.priority = Priority.High;
        this.Prefix!.before = new[] { LigoModule.Arsenal.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("Ligo.Modules.Arsenal")]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        if (ModEntry.Config.EnableArsenal)
        {
            return true; // hand over to Slingshots module
        }

        try
        {
            if (__instance.attachments[0] is null)
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
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            // get and spend ammo
            var ammo = __instance.attachments[0].getOne();
            if (--__instance.attachments[0].Stack <= 0)
            {
                __instance.attachments[0] = null;
            }

            int damageBase;
            float knockback;
            switch (ammo.ParentSheetIndex)
            {
                case SObject.wood or SObject.coal:
                    damageBase = 2;
                    knockback = 0.4f;
                    break;
                case SObject.stone:
                    damageBase = 5;
                    knockback = 0.5f;
                    break;
                case SObject.copper:
                    damageBase = 10;
                    knockback = 0.55f;
                    break;
                case SObject.iron:
                    damageBase = 20;
                    knockback = 0.60f;
                    break;
                case SObject.gold:
                    damageBase = 30;
                    knockback = 0.65f;
                    break;
                case SObject.iridium:
                    damageBase = 50;
                    knockback = 0.70f;
                    break;
                case Constants.ExplosiveAmmoIndex:
                    damageBase = 5;
                    knockback = 5f;
                    break;
                case Constants.SlimeIndex:
                    damageBase = who.HasProfession(Profession.Piper) ? 10 : 5;
                    knockback = 0f;
                    break;
                default: // fish, fruit or vegetable
                    damageBase = 1;
                    knockback = 0f;
                    break;
            }

            // apply slingshot damage modifiers
            float damageMod;
            switch (__instance.InitialParentTileIndex)
            {
                case Constants.MasterSlingshotIndex:
                    damageMod = 1.5f;
                    knockback += 0.1f;
                    break;
                case Constants.GalaxySlingshotIndex:
                    damageMod = 2f;
                    knockback += 0.2f;
                    break;
                default:
                    damageMod = 1f;
                    break;
            }

            // calculate overcharge
            var overcharge = who.HasProfession(Profession.Desperado) ? __instance.GetOvercharge(who) : 1f;

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
            var index = ammo.ParentSheetIndex;
            if (ammo.ParentSheetIndex is not (Constants.ExplosiveAmmoIndex or Constants.SlimeIndex
                    or Constants.RadioactiveOreIndex) && damageBase > 1)
            {
                ammo.ParentSheetIndex++;
            }

            var projectile = new ObjectProjectile(
                    ammo,
                    index,
                    __instance,
                    who,
                    damage,
                    knockback,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity);

            if (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
            {
                projectile.IgnoreLocationCollision = true;
            }

            location.projectiles.Add(projectile);

            // do Death Blossom
            if (who.Get_Ultimate() is DeathBlossom { IsActive: true })
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
                    var petal = new ObjectProjectile(
                        ammo,
                        index,
                        __instance,
                        who,
                        damage,
                        knockback,
                        0f,
                        startingPosition,
                        velocity.X,
                        velocity.Y,
                        rotationVelocity);

                    if (Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null)
                    {
                        petal.IgnoreLocationCollision = true;
                    }

                    who.currentLocation.projectiles.Add(petal);
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
