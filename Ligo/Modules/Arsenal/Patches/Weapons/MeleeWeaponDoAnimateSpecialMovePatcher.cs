namespace DaLion.Ligo.Modules.Arsenal.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
        this.Postfix!.before = new[] { LigoModule.Rings.Namespace };
    }

    #region harmony patches

    /// <summary>Implement Garnet enchantment CDR.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("Ligo.Modules.Rings")]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var cdr = 1f - ((__instance.GetEnchantmentLevel<GarnetEnchantment>() +
                         __instance.Read<float>(DataFields.ResonantCooldownReduction)) * 0.1f);
        if (cdr <= 0f)
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * cdr);
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * cdr);
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * cdr);
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * cdr);
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
                            .MakeGenericMethod(typeof(ReduxArtfulEnchantment))),
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
