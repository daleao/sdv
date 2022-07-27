namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Reflection;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : Common.Harmony.HarmonyPatch
{
    private static Action<Slingshot>? _UpdateAimPos;

    /// <summary>Construct an instance.</summary>
    internal SlingshotPerformFirePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.PerformFire));
        Prefix!.priority = Priority.High;
        Prefix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Patch to add Rascal bonus range damage + perform Desperado perks and Ultimate.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            if (__instance.get_IsOnSpecial()) return false; // don't run original logic

            if (ModEntry.IsImmersiveProfessionsLoaded) return true; // pass on to Immersive Professions

            if (__instance.attachments[0] is null)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
                return false; // don't run original logic

            _UpdateAimPos ??= typeof(Slingshot).RequireMethod("updateAimPos")
                .CompileUnboundDelegate<Action<Slingshot>>();
            _UpdateAimPos(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (x, y) = StardewValley.Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

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

            BasicProjectile.onCollisionBehavior? collisionBehavior;
            string collisionSound;
            switch (ammo.ParentSheetIndex)
            {
                case 441:
                    collisionBehavior = BasicProjectile.explodeOnImpact;
                    collisionSound = "explosion";
                    break;
                case 909:
                    collisionBehavior = null;
                    collisionSound = "hammer";
                    break;
                default:
                    collisionBehavior = null;
                    collisionSound = ammo.IsSquishyAmmo() ? "slimedead" : "hammer";
                    if (damageBase > 1) ++ammo.ParentSheetIndex;

                    break;
            }

            var damageMod = __instance.InitialParentTileIndex switch
            {
                33 => 2f,
                34 => 4f,
                _ => 1f
            } * (1f + __instance.GetEnchantmentLevel<RubyEnchantment>() + who.attackIncreaseModifier);

            if (Game1.options.useLegacySlingshotFiring)
            {
                x *= -1f;
                y *= -1f;
            }

            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var damage = (damageBase + Game1.random.Next(-damageBase / 2, damageBase + 2)) * damageMod;
            var projectile = new ImmersiveProjectile(__instance, (int)damage, ammo.ParentSheetIndex, 0, 0,
                (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition,
                collisionSound, "", false, true, location, who, true, collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null
            };
            location.projectiles.Add(projectile);

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