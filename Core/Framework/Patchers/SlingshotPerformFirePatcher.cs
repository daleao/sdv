namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Reflection;
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

    /// <summary>Do Snowball shot.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("DaLion.Professions")]
    [UsedImplicitly]
    private static bool SlingshotPerformFirePrefix(
        Slingshot __instance, ref bool ___canPlaySound, GameLocation location, Farmer who)
    {
        try
        {
            if (__instance.attachments[0] is not null || !who.IsStandingOnSnow())
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
            var projectile = new SnowballProjectile(
                startingPosition,
                xVelocity,
                yVelocity,
                rotationVelocity);

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
