namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using Buff = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoAnimateSpecialMovePatcher"/> class.</summary>
    internal MeleeWeaponDoAnimateSpecialMovePatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>("doAnimateSpecialMove");
    }

    #region harmony patches

    /// <summary>Prevent special moves while Jinxed.</summary>
    [HarmonyPrefix]
    private static bool MeleeWeaponDoAnimateSpecialMovePrefix(MeleeWeapon __instance)
    {
        if (__instance.getLastFarmerToUse() is not { } user || user.CurrentTool != __instance)
        {
            return false; // don't run original logic
        }

        return !CombatModule.Config.EnableStatusConditions || !user.hasBuff((int)Buff.Jinxed); // conditionally run original logic
    }

    /// <summary>Implement Garnet enchantment CDR.</summary>
    [HarmonyPostfix]
    private static void MeleeWeaponDoAnimateSpecialMovePostfix(MeleeWeapon __instance)
    {
        var lastUser = __instance.getLastFarmerToUse();
        var cooldownReduction = __instance.Get_EffectiveCooldownReduction() * lastUser.Get_CooldownReduction();
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

    /// <summary>Increase hit count of empowered dagger's special stab move.</summary>
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
                                .MakeGenericMethod(typeof(InfinityEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, notInfinity),
                        new CodeInstruction(OpCodes.Ldc_I4_6),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding empowered dagger effect.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
