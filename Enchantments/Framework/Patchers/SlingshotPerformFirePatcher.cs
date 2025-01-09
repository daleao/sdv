namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Core;
using DaLion.Core.Framework.Enchantments;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Enchantments.Framework.Projectiles;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;
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

    /// <summary>Do Quincy shot.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [UsedImplicitly]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            var canDoQuincy = __instance.hasEnchantmentOfType<QuincyEnchantment>() && CoreMod.State.AreEnemiesNearby;
            if (__instance.attachments[0] is not null || !canDoQuincy)
            {
                return true; // run original logic
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

            // adjust velocity
            if (Game1.options.useLegacySlingshotFiring)
            {
                xVelocity *= -1f;
                yVelocity *= -1f;
            }

            // add main projectile
            var startingPosition = shootOrigin - new Vector2(32f, 32f);
            var rotationVelocity = (float)(Math.PI / (64f + Game1.random.Next(-63, 64)));
            var projectile = new QuincyProjectile(
                who,
                startingPosition,
                xVelocity,
                yVelocity,
                rotationVelocity,
                1f);

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

    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? SlingshotPerformFireTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: OnFire(projectile, this, location, who);
        // After: location.projectiles.Add( ... );
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetCollection<Projectile>).RequireMethod(nameof(NetCollection<Projectile>.Add))),
                    new CodeInstruction(OpCodes.Br_S),
                ])
                .Insert([
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, typeof(SlingshotPerformFirePatcher).RequireMethod(nameof(OnFire))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting slingshot enchantment OnFire callback.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void OnFire(BasicProjectile firedProjectile, Slingshot instance, GameLocation location, Farmer firer)
    {
        foreach (var enchantment in instance.enchantments.OfType<BaseSlingshotEnchantment>())
        {
            enchantment.OnFire(instance, firedProjectile, location, firer);
        }
    }

    #endregion injections
}
