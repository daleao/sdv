namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatch()
    {
        Target = RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    /// <summary>Doubles hit count of Infinity Dagger's special stab move.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: daggerHitsLeft = 4;
        /// To: daggerHitsLeft = this.BaseName.Contains "Infinity" ? 8 : 4;

        var notInfinity = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldc_I4_4)
                )
                .AddLabels(notInfinity)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(MeleeWeapon).RequireMethod(nameof(MeleeWeapon.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(InfinityEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                    new CodeInstruction(OpCodes.Ldc_I4_8),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding infinity dagger effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}