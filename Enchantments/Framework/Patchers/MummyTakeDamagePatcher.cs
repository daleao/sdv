namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MummyTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MummyTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MummyTakeDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Mummy>(
            nameof(Mummy.takeDamage),
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)]);
    }

    #region harmony patches

    /// <summary>Crusader effect for Sunburst enchantment.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MummyTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (weapon.hasEnchantmentOfType<CrusaderEnchantment>())
        // To: if (weapon.hasEnchantmentOfType<CrusaderEnchantment>() || weapon.hasEnchantmentOfType<SunburstEnchantment>())
        try
        {
            var makeJucier = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(CrusaderEnchantment))),
                ])
                .Move()
                .GetOperand(out var resumeExecution)
                .ReplaceWith(new CodeInstruction(OpCodes.Brtrue_S, makeJucier)) // we are changing AND to OR
                .Move()
                .AddLabels(makeJucier)
                .Insert([
                    // or meleeWeapon.hasEnchantmentOfType<SunburstEnchantment>()
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(SunburstEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Sunburst Crusader effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
