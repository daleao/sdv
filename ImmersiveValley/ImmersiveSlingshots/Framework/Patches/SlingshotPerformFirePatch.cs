namespace DaLion.Stardew.Slingshots.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Integrations.WalkOfLife;
using Enchantments;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Reflection;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotPerformFirePatch : Common.Harmony.HarmonyPatch
{
    private static readonly Lazy<Action<Slingshot>> _UpdateAimPos = new(() =>
        typeof(Slingshot).RequireMethod("updateAimPos").CompileUnboundDelegate<Action<Slingshot>>());

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
    private static bool SlingshotPerformFirePrefix(Slingshot __instance, ref int? __state, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            __state = __instance.attachments[0]?.Stack;
            if (__instance.get_IsOnSpecial()) return false; // don't run original logic

            var hasQuincyEnchantment = __instance.hasEnchantmentOfType<QuincyEnchantment>();
            if (ModEntry.ProfessionsApi is not null && (__instance.attachments[0] is not null || hasQuincyEnchantment))
                return true; // hand over to Immersive Professions

            if (__instance.attachments[0] is null && !hasQuincyEnchantment && !who.IsSteppingOnSnow())
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));
                ___canPlaySound = true;
                return false; // don't run original logic
            }

            var backArmDistance = __instance.GetBackArmDistance(who);
            if (backArmDistance <= 4 || ___canPlaySound)
                return false; // don't run original logic

            _UpdateAimPos.Value(__instance);
            var mouseX = __instance.aimPos.X;
            var mouseY = __instance.aimPos.Y;
            var shootOrigin = __instance.GetShootOrigin(who);
            var (x, y) = StardewValley.Utility.getVelocityTowardPoint(shootOrigin, __instance.AdjustForHeight(new(mouseX, mouseY)),
                (15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));

            var ammo = __instance.attachments[0]?.getOne();
            if (ammo is not null && --__instance.attachments[0].Stack <= 0)
                __instance.attachments[0] = null;

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
                null => hasQuincyEnchantment ? 5 : 1, // quincy or snowball
                _ => 1 // fish, fruit or vegetable
            };

            BasicProjectile.onCollisionBehavior? collisionBehavior;
            string collisionSound;
            switch (ammo?.ParentSheetIndex)
            {
                case 441:
                    collisionBehavior = BasicProjectile.explodeOnImpact;
                    collisionSound = "explosion";
                    break;
                case 909:
                    collisionBehavior = null;
                    collisionSound = "hammer";
                    break;
                case null:
                    collisionBehavior = null;
                    collisionSound = hasQuincyEnchantment ? "debuffHit" : "snowyStep";
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
            var index = ammo?.ParentSheetIndex ?? (__instance.hasEnchantmentOfType<QuincyEnchantment>()
                ? Constants.QUINCY_PROJECTILE_INDEX_I
                : Constants.SNOWBALL_PROJECTILE_INDEX_I);
            var projectile = new ImmersiveProjectile(__instance, (int)damage, index, 0,
                index == Constants.QUINCY_PROJECTILE_INDEX_I ? 5 : 0,
                (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), x, y, startingPosition, collisionSound, index == Constants.QUINCY_PROJECTILE_INDEX_I ? "debuffSpell" : "",
                false, index != Constants.SNOWBALL_PROJECTILE_INDEX_I, location, who, ammo is not null,
                collisionBehavior)
            {
                IgnoreLocationCollision = Game1.currentLocation.currentEvent != null || Game1.currentMinigame != null,
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

    /// <summary>Perform <see cref="BaseSlingshotEnchantment.OnFire"/> action.</summary>
    [HarmonyPostfix]
    private static void SlingshotPerformFirePostfix(Slingshot __instance, int? __state, GameLocation location, Farmer who)
    {
        if (__state is not null && __instance.attachments[0].Stack == __state || location.projectiles.Count <= 0) return;

        var ultimate = ModEntry.ProfessionsApi?.GetRegisteredUltimate();
        if (ultimate is not null && ultimate.Index == IImmersiveProfessions.UltimateIndex.Blossom &&
            ultimate.IsActive) return;

        var projectile = location.projectiles[^1];
        var typeName = projectile.GetType().FullName;
        if (typeName is null || !typeName.Contains("DaLion") || !typeName.Contains("ImmersiveProjectile")) return;

        foreach (var enchantment in __instance.enchantments.OfType<BaseSlingshotEnchantment>())
            enchantment.OnFire(__instance, (BasicProjectile)projectile, location, who);
    }

    #endregion harmony patches
}