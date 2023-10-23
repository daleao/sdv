namespace DaLion.Overhaul.Modules.Combat.Patchers.Ranged;

#region using directives

using System.Reflection;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
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
        try
        {
            var canDoQuincy = __instance.hasEnchantmentOfType<QuincyEnchantment>() && location.HasMonsters();
            if (__instance.attachments[0] is null && !canDoQuincy && !who.IsSteppingOnSnow())
            {
                if (__instance.attachments.Length > 1 && __instance.attachments[1] is not null &&
                    __instance.attachments[1].ParentSheetIndex != ObjectIds.MonsterMusk)
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
            if (ammo is not null && --__instance.attachments[0].Stack <= 0)
            {
                __instance.attachments[0] = null;
            }

            // set projectile index
            var index = ammo?.ParentSheetIndex ?? (canDoQuincy
                ? QuincyProjectile.BlueTileSheetIndex
                : Projectile.snowBall);

            // calculate overcharge
            var overcharge = ProfessionsModule.ShouldEnable && who.professions.Contains(Farmer.desperado)
                ? __instance.GetOvercharge()
                : 1f;
            if (overcharge > 1f)
            {
                EventManager.Disable<DesperadoUpdateTickedEvent>();
            }

            // adjust velocity
            if (Game1.options.useLegacySlingshotFiring)
            {
                xVelocity *= -1f;
                yVelocity *= -1f;
            }

            if (__instance.GetEnchantmentOfType<RangedEnergizedEnchantment>() is
                { Energy: >= RangedEnergizedEnchantment.MaxEnergy })
            {
                SoundEffectPlayer.PlasmaShot.Play();
            }

            // add main projectile
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
            if (ammo?.ParentSheetIndex is ObjectIds.Wood or ObjectIds.Coal or ObjectIds.Stone or ObjectIds.CopperOre
                or ObjectIds.IronOre or ObjectIds.GoldOre or ObjectIds.IridiumOre)
            {
                index++;
            }

            BasicProjectile projectile = index switch
            {
                QuincyProjectile.BlueTileSheetIndex => new QuincyProjectile(
                    __instance,
                    who,
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

                    rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
                    BasicProjectile petal = index switch
                    {
                        QuincyProjectile.BlueTileSheetIndex => new QuincyProjectile(
                            __instance,
                            who,
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
                    overcharge,
                    startingPosition,
                    xVelocity,
                    yVelocity,
                    rotationVelocity,
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
