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
        /// To: int actualDamage = this.get_GotCrit() && ModEntry.Config.CritsIgnoreDefense ? damage : damage * 10 / (10 + (int)resilience) ;

        var mitigateDamage = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call, typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.CritsIgnoreDefense))),
                    new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.get_GotCrit))),
                    new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Stloc_0),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .RemoveInstructions()
                .AddLabels(mitigateDamage)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 10f),
                    new CodeInstruction(OpCodes.Ldc_R4, 10f)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Sub)
                )
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Div)
                )
                .ReplaceInstructionWith(new(OpCodes.Mul))
                .Advance()
                .ReplaceInstructionWith(new(OpCodes.Conv_I4))
                .Advance(2)
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