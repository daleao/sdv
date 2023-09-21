namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Apply Topaz bonus for Slingshots.</summary>
    [HarmonyPrefix]
    private static void FarmerTakeDamagePrefix(Farmer __instance, ref bool __state)
    {
        __state = false;
        if (__instance.CurrentTool is not Slingshot slingshot || !slingshot.hasEnchantmentOfType<TopazEnchantment>() ||
            !CombatModule.Config.RebalancedGemstones)
        {
            return;
        }

        if (CombatModule.ShouldEnable && CombatModule.Config.NewResistanceFormula)
        {
            return;
        }

        __instance.resilience += slingshot.GetEnchantmentLevel<TopazEnchantment>();
        __state = true;
    }

    /// <summary>Apply Topaz bonus for Slingshots.</summary>
    [HarmonyPostfix]
    private static void FarmerTakeDamagePostfix(Farmer __instance, bool __state)
    {
        if (__state)
        {
            __instance.resilience -= __instance.CurrentTool.GetEnchantmentLevel<TopazEnchantment>();
        }
    }

    /// <summary>Trigger damage taken effects.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: OnDamageTaken(this, damage);
        // Before: this.temporarilyInvincible = true;
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4_1), // 1 is for true
                        new CodeInstruction(
                            OpCodes.Stfld,
                            typeof(Farmer).RequireField(nameof(Farmer.temporarilyInvincible))),
                    })
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1), // int damage
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(OnDamageTaken))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 2).\nHelper returned {ex}");
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
            explosive.Accumulated += damage / 2;
            if (explosive.ExplosionRadius >= 1)
            {
                EventManager.Enable<ExplosiveUpdateTickedEvent>();
            }
        }
    }

    #endregion injected subroutines
}
