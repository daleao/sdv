namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<GreenSlime>([typeof(Vector2), typeof(int)]);
    }

    #region harmony patches

    /// <summary>Patch to add Slime IVs.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeCtorPostfix(GreenSlime __instance)
    {
        var r = new Random(Guid.NewGuid().GetHashCode());

        // Record base Health
        var baseHealth = __instance.Health;
        if (__instance.hasSpecialItem.Value)
        {
            baseHealth /= 3;
        }

        if (__instance.cute.Value)
        {
            baseHealth = (int)Math.Round(baseHealth / 1.25f);
        }

        Data.Write(__instance, DataKeys.BaseHealth, baseHealth.ToString());

        // Choose Health IV
        var healthIV = r.Next(3);
        __instance.Health = (int)(__instance.Health * (1f + (healthIV / 5f)));
        __instance.MaxHealth = __instance.Health;
        Data.Write(__instance, DataKeys.HealthIV, healthIV.ToString());

        // Record Base Attack
        var baseAttack = __instance.DamageToFarmer;
        if (__instance.hasSpecialItem.Value)
        {
            baseAttack /= 2;
        }

        if (__instance.cute.Value)
        {
            baseAttack--;
        }

        Data.Write(__instance, DataKeys.BaseAttack, baseAttack.ToString());

        // Choose Attack IV
        var attackIV = r.Next(3);
        __instance.DamageToFarmer = (int)(__instance.DamageToFarmer * (1f + (attackIV / 5f)));
        Data.Write(__instance, DataKeys.AttackIV, attackIV.ToString());

        // Record base Defense
        Data.Write(__instance, DataKeys.BaseHealth, __instance.resilience.Value.ToString());

        // Choose Defense IV
        var defenseIV = r.Next(3);
        __instance.resilience.Value += defenseIV;
        Data.Write(__instance, DataKeys.DefenseIV, defenseIV.ToString());
    }

    #endregion harmony patches
}
