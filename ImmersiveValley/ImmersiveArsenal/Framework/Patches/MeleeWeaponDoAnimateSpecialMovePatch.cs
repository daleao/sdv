namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Stardew.Arsenal.Framework.Enchantments;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatch"/> class.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatch()
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
        this.Postfix!.before = new[] { "DaLion.ImmersiveRings" };
    }

    #region harmony patches

    /// <summary>Implement Topaz enchantment CDR.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.ImmersiveRings")]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var cdr = __instance.GetEnchantmentLevel<GarnetEnchantment>() * 0.1f;
        if (cdr <= 0f)
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * (1f - cdr));
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * (1f - cdr));
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * (1f - cdr));
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * (1f - cdr));
        }
    }

    /// <summary>Increase hit count of Infinity Dagger's special stab move.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: daggerHitsLeft = 4;
        // To: daggerHitsLeft = this.BaseName.Contains "Infinity" ? 6 : 4;
        try
        {
            var notInfinity = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindLast(new CodeInstruction(OpCodes.Ldc_I4_4))
                .AddLabels(notInfinity)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeapon)
                            .RequireMethod(nameof(MeleeWeapon.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(InfinityEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                    new CodeInstruction(OpCodes.Ldc_I4_6),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
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
