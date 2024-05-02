namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
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
    internal SlingshotPerformFirePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and LimitBreak.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.Combat")]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        // !! COMBAT INTERVENTION HERE

        try
        {
            var hasSecondaryAmmo = __instance.attachments.Length > 1 && __instance.attachments[1] is not null;
            if (__instance.attachments[0] is null)
            {
                if (hasSecondaryAmmo && __instance.attachments[1].QualifiedItemId != QualifiedObjectIds.MonsterMusk)
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
            var ammo = (SObject)__instance.attachments[0].getOne();
            if (--__instance.attachments[0].Stack <= 0)
            {
                __instance.attachments[0] = null;
            }

            // calculate damage
            var damageBase = __instance.GetAmmoDamage(ammo);
            var damageMod = __instance.QualifiedItemId switch
            {
                QualifiedWeaponIds.MasterSlingshot => 2f,
                QualifiedWeaponIds.GalaxySlingshot => 4f,
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
            if (!isLimitActive && Config.ModKey.IsDown() && hasSecondaryAmmo && __instance.attachments[1] is
                    { QualifiedItemId: QualifiedObjectIds.MonsterMusk } musk)
            {
                var uses = Data.ReadAs(musk, DataKeys.MuskUses, 10);
                if (--uses <= 0)
                {
                    __instance.attachments[1] = null;
                }

                isMusked = true;
                Data.Write(musk, DataKeys.MuskUses, uses.ToString());
            }

            var projectile = new ObjectProjectile(
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
                    var petal = new ObjectProjectile(
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

            // do Prestiged Rascsal double shot
            if (!isLimitActive && who.HasProfession(Profession.Rascal, true) && Config.ModKey.IsDown() &&
                hasSecondaryAmmo && !isMusked)
            {
                // get and spend ammo
                ammo = (SObject)__instance.attachments[1].getOne();
                if (--__instance.attachments[1].Stack <= 0)
                {
                    __instance.attachments[1] = null;
                }

                // calculate damage
                damageBase = __instance.GetAmmoDamage(ammo);
                damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                var secondProjectile = new ObjectProjectile(
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

                Game1.delayedActions.Add(new DelayedAction(200, () => location.projectiles.Add(secondProjectile)));
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
    private static void SlingshotPerformFirePostfix(Slingshot __instance)
    {
        if (__instance.CanAutoFire())
        {
            __instance.pullStartTime = Game1.currentGameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    #endregion harmony patches
}
