namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Enchantments.Melee;
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

    /// <summary>Overhaul for farmer defense.</summary>
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

        var tribute = weapon.GetEnchantmentOfType<TributeEnchantment>();
        if (tribute is not null)
        {
            tribute.Threshold = 0.1f;
        }

        var exploding = weapon.GetEnchantmentOfType<ExplodingEnchantment>();
        if (exploding is not null && !weapon.isOnSpecial)
        {
            exploding.Accumulated += damage;
        }
    }

    #endregion injected subroutines
}
