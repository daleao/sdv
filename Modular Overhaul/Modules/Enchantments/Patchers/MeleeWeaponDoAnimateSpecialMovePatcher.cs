namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
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
        this.Postfix!.before = new[] { OverhaulModule.Rings.Namespace };
    }

    #region harmony patches

    /// <summary>Implement Garnet enchantment CDR.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("Overhaul.Modules.Rings")]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var cooldownReduction = WeaponsModule.ShouldEnable
            ? __instance.Get_EffectiveCooldownReduction()
            : 1f - (__instance.GetEnchantmentLevel<GarnetEnchantment>() * 0.1f);
        if (Math.Abs(cooldownReduction - 1f) < 0.01f)
        {
            return;
        }

        if (MeleeWeapon.attackSwordCooldown > 0)
        {
            MeleeWeapon.attackSwordCooldown = (int)(MeleeWeapon.attackSwordCooldown * cooldownReduction);
        }

        if (MeleeWeapon.defenseCooldown > 0)
        {
            MeleeWeapon.defenseCooldown = (int)(MeleeWeapon.defenseCooldown * cooldownReduction);
        }

        if (MeleeWeapon.daggerCooldown > 0)
        {
            MeleeWeapon.daggerCooldown = (int)(MeleeWeapon.daggerCooldown * cooldownReduction);
        }

        if (MeleeWeapon.clubCooldown > 0)
        {
            MeleeWeapon.clubCooldown = (int)(MeleeWeapon.clubCooldown * cooldownReduction);
        }
    }

    /// <summary>Increase hit count of Artful dagger's special stab move.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: daggerHitsLeft = 4;
        // To: daggerHitsLeft = this.hasEnchantmentOfType<NewArtfulEnchantment>() ? 5 : 4;
        try
        {
            var notInfinity = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) }, ILHelper.SearchOption.Last)
                .AddLabels(notInfinity)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MeleeWeapon)
                                .RequireMethod(nameof(MeleeWeapon.hasEnchantmentOfType))
                                .MakeGenericMethod(typeof(MeleeArtfulEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                        new CodeInstruction(OpCodes.Ldc_I4_6),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Artful dagger effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
