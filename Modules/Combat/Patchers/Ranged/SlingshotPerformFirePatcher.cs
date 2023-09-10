namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
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
        this.Prefix!.after = new[] { OverhaulModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyAfter("DaLion.Overhaul.Modules.Professions")]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance,
        ref bool ___canPlaySound,
        GameLocation location,
        Farmer who)
    {
        if (__instance.Get_IsOnSpecial())
        {
            return false; // don't run original logic
        }

        try
        {
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
            Reflector
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
            int ammoDamage;
            if (ammo is not null)
            {
                if (!__instance.hasEnchantmentOfType<PreservingEnchantment>() ||
                    ammo.ParentSheetIndex == SObject.prismaticShardIndex)
                {
                    if (--__instance.attachments[0].Stack <= 0)
                    {
                        __instance.attachments[0] = null;
                    }
                }

                ammoDamage = __instance.GetAmmoDamage();
            }
            else
            {
                ammoDamage = canDoQuincy ? 25 : 1;
            }

            // apply slingshot damage modifiers
            var damageMod = 1f;
            var knockback = 0.25f;
            switch (__instance.InitialParentTileIndex)
            {
                case ItemIDs.MasterSlingshot:
                    damageMod = CombatModule.Config.EnableWeaponOverhaul ? 0.5f : 1f;
                    knockback += 0.1f;
                    break;
                case ItemIDs.GalaxySlingshot:
                    damageMod = CombatModule.Config.EnableWeaponOverhaul ? 1f : CombatModule.Config.EnableInfinitySlingshot ? 2f : 3f;
                    knockback += 0.2f;
                    break;
                case ItemIDs.InfinitySlingshot:
                    damageMod = CombatModule.Config.EnableWeaponOverhaul ? 1.5f : 3f;
                    knockback += 0.25f;
                    break;
            }

            // set projectile index
            var index = ammo?.ParentSheetIndex ?? (canDoQuincy
                ? QuincyProjectile.TileSheetIndex
                : Projectile.snowBall);

            // calculate overcharge
            var overcharge = ProfessionsModule.ShouldEnable && who.professions.Contains(Farmer.desperado)
                ? __instance.GetOvercharge()
                : 1f;

            // adjust velocity
            if (overcharge > 1f)
            {
                if (index != QuincyProjectile.TileSheetIndex)
                {
                    xVelocity *= overcharge;
                    yVelocity *= overcharge;
                }

                EventManager.Disable<DesperadoUpdateTickedEvent>();
            }

            if (Game1.options.useLegacySlingshotFiring)
            {
                xVelocity *= -1f;
                yVelocity *= -1f;
            }

            // add main projectile
            var damage = (ammoDamage + Game1.random.Next(-ammoDamage / 2, ammoDamage + 2)) * damageMod;
            knockback = CombatModule.Config.EnableWeaponOverhaul ? knockback : 1f;
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
            if (ammo?.ParentSheetIndex is SObject.wood or SObject.coal or SObject.stone or SObject.copper
                or SObject.iron or SObject.gold or SObject.iridium)
            {
                index++;
            }

            BasicProjectile projectile = index switch
            {
                QuincyProjectile.TileSheetIndex => new QuincyProjectile(
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
                    knockback,
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity),
            };

            if (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
            {
                projectile.IgnoreLocationCollision = true;
            }

            if (__instance.hasEnchantmentOfType<MagnumEnchantment>())
            {
                projectile.startingScale.Value *= 2f;
            }

            location.projectiles.Add(projectile);
            ___canPlaySound = true;

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

                    damage = (ammoDamage + Game1.random.Next(-ammoDamage / 2, ammoDamage + 2)) * damageMod;
                    rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                    BasicProjectile petal = index switch
                    {
                        QuincyProjectile.TileSheetIndex => new QuincyProjectile(
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
                            knockback,
                            0f,
                            startingPosition,
                            velocity.X,
                            velocity.Y,
                            rotationVelocity),
                    };

                    if (Game1.currentLocation.currentEvent is not null || Game1.currentMinigame is not null)
                    {
                        petal.IgnoreLocationCollision = true;
                    }

                    who.currentLocation.projectiles.Add(petal);
                }
            }

            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                if (__instance.enchantments[i] is not BaseSlingshotEnchantment slingshotEnchantment)
                {
                    continue;
                }

                slingshotEnchantment.OnFire(
                    __instance,
                    projectile,
                    ammoDamage,
                    damageMod,
                    knockback,
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
