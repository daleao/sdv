namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Xna;
using Events.GameLoop;
using Extensions;
using HarmonyLib;
using LinqFasterer;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Reflection;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static readonly Lazy<Action<Slingshot>> _UpdateAimPos = new(() =>
        typeof(Slingshot).RequireMethod("updateAimPos").CompileUnboundDelegate<Action<Slingshot>>());

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        Prefix!.priority = Priority.High;
        Prefix!.after = new[] { "DaLion.ImmersiveSlingshots" };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix, HarmonyPriority(Priority.High), HarmonyAfter("DaLion.ImmersiveSlingshots")]
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, ref bool ___canPlaySound,
        GameLocation location, Farmer who)
    {
        try
        {
            var usingPrimaryAmmo = ModEntry.State.UsingPrimaryAmmo;
            var usingSecondaryAmmo = ModEntry.State.UsingSecondaryAmmo;
            var ammoIndex = usingPrimaryAmmo ? 0 : 1;
            var hasQuincyEnchantment = __instance.enchantments.FirstOrDefaultF(e =>
                e.GetType().FullName?.ContainsAllOf("ImmersiveSlingshots", "QuincyEnchantment") == true) is not null;
            var hasPreservingEnchantment = __instance.enchantments.FirstOrDefaultF(e =>
                e.GetType().FullName?.ContainsAllOf("ImmersiveSlingshots", "PreservingEnchantment") == true) is not null;
            if (__instance.attachments[ammoIndex] is null && !hasQuincyEnchantment)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
                return false; // don't run original logic

            // calculate projectile velocity
            _UpdateAimPos.Value(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (x, y) = StardewValley.Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            // get and spend ammo instance
            var ammo = __instance.attachments[ammoIndex]?.getOne();
            var didPreserve = hasPreservingEnchantment && Game1.random.NextDouble() < 0.5 + who.DailyLuck / 2 + who.LuckLevel * 0.01;
            if (ammo is not null && !didPreserve && --__instance.attachments[ammoIndex].Stack <= 0)
                __instance.attachments[ammoIndex] = null;

            var damageBase = ammo?.ParentSheetIndex switch
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
                766 => who.HasProfession(Profession.Piper, true) ? 10 : 5, // slime
                null => 5, // quincy
                _ => 1 // fish, fruit or vegetable
            };

            // set collision properties
            BasicProjectile.onCollisionBehavior? primaryCollisionBehavior = null;
            string primaryCollisionSound = string.Empty;
            switch (ammo?.ParentSheetIndex)
            {
                case 441:
                    primaryCollisionBehavior = BasicProjectile.explodeOnImpact;
                    primaryCollisionSound = "explosion";
                    break;
                case 909 or 766:
                    primaryCollisionSound = ammoIndex == 766 ? "slimedead" : "hammer";
                    break;
                case null:
                    primaryCollisionSound = "debuffHit";
                    break;
                default:
                    primaryCollisionSound = ammo.IsSquishyAmmo() ? "slimedead" : "hammer";
                    if (damageBase > 1) ++ammoIndex;

                    break;
            }

            // apply slingshot damage modifiers
            var damageMod = __instance.InitialParentTileIndex switch
            {
                33 => 2f,
                34 => 4f,
                _ => 1f
            } * (1f + __instance.GetEnchantmentLevel<RubyEnchantment>() + who.attackIncreaseModifier);

            // calculate overcharge
            var overcharge = 1f;
            if (who.HasProfession(Profession.Desperado) && !__instance.CanAutoFire())
                overcharge += __instance.GetOvercharge(who);

            // adjust velocity
            if (overcharge > 1f)
            {
                x *= overcharge;
                y *= overcharge;
                ModEntry.Events.Disable<DesperadoUpdateTickedEvent>();
            }

            if (Game1.options.useLegacySlingshotFiring)
            {
                x *= -1f;
                y *= -1f;
            }

            // calculate bounces
            var bounces = 0;
            if (who.HasProfession(Profession.Rascal) && ammo?.IsMineralAmmo() == true &&
                ModEntry.Config.ModKey.IsDown())
            {
                ++bounces;
                damageMod *= 0.6f;
            }

            // add main projectile
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod * overcharge;
            var projectile = new ImmersiveProjectile(__instance, overcharge, !didPreserve, (int)damage, ammo?.ParentSheetIndex ?? 14,
                bounces, ammo is null ? 5 : 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition,
                primaryCollisionSound, ammo is null ? "debuffSpell" : "", false, true, location, who, ammo is not null, primaryCollisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null
            };

            if (ammo is null) projectile.startingScale.Value *= overcharge * overcharge;

            location.projectiles.Add(projectile);

            // add auxiliary projectiles
            var velocity = new Vector2(x, y);
            var speed = velocity.Length();
            velocity.Normalize();
            if (who.IsLocalPlayer && who.get_Ultimate() is DeathBlossom { IsActive: true })
            {
                // do Death Blossom
                for (var i = 0; i < 7; ++i)
                {
                    velocity = velocity.Rotate(45);
                    if (i == 3) continue;

                    damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
                    var petal = new ImmersiveProjectile(__instance, 1f, false, (int)damage, ammo?.ParentSheetIndex ?? 14, 0,
                        ammo is null ? 5 : 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), velocity.X * speed,
                        velocity.Y * speed, startingPosition, primaryCollisionSound, string.Empty, false, true, location, who,
                        ammo is not null, primaryCollisionBehavior)
                    {
                        IgnoreLocationCollision = Game1.currentLocation.currentEvent is not null ||
                                                  Game1.currentMinigame is not null
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