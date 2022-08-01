namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterTakeDamagePatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Crits ignore defense, which, btw, actually does something.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterTakeDamageTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int actualDamage = Math.Max(1, damage - (int)resilience);
        /// To: int actualDamage = this.get_GotCrit() ? damage : Math.Max(1, damage - (int)resilience * (int)resilience);

        var didntGetCrit = generator.DefineLabel();
        var dontSquareDefense = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call, typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.CritsIgnoreDefense))),
                    new CodeInstruction(OpCodes.Brfalse_S, didntGetCrit),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.get_GotCrit))),
                    new CodeInstruction(OpCodes.Brfalse_S, didntGetCrit),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Stloc_0),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .AddLabels(didntGetCrit)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Sub)
                )
                .AddLabels(dontSquareDefense)
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call, typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.ImprovedEnemyDefense))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontSquareDefense),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Mul)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0)
                )
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding defense options.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}