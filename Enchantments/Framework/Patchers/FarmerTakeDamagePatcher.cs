namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Enchantments.Framework.Events;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerTakeDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Grant i-frames during Stabbing Sword lunge attack.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerTakeDamagePrefix(Farmer __instance)
    {
        return __instance.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.stabbingSword, isOnSpecial: true };
    }

    /// <summary>Trigger damage taken effects.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: OnDamageTaken(this, damage);
        // Before: this.temporarilyInvincible = true;
        try
        {
            helper
                .PatternMatch(
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_1), // 1 is for true
                    new CodeInstruction(
                        OpCodes.Stfld,
                        typeof(Farmer).RequireField(nameof(Farmer.temporarilyInvincible))),
                ])
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1), // int damage
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(OnDamageTaken))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injected damage taken enchantment effects.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void OnDamageTaken(Farmer farmer, int damage)
    {
        if (farmer.CurrentTool is not MeleeWeapon weapon)
        {
            return;
        }

        var mammon = weapon.GetEnchantmentOfType<MammoniteEnchantment>();
        if (mammon is not null)
        {
            mammon.Threshold = 0.1f;
            return;
        }

        var explosive = weapon.GetEnchantmentOfType<ExplosiveEnchantment>();
        if (explosive is not null && !weapon.isOnSpecial)
        {
            explosive.Accumulated += damage * 2;
            if (explosive.ExplosionRadius >= 1)
            {
                EventManager.Enable<ExplosiveUpdateTickedEvent>();
            }
        }
    }

    #endregion injected subroutines
}
