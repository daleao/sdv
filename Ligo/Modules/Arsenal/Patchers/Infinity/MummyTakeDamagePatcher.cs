namespace DaLion.Ligo.Modules.Arsenal.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MummyTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MummyTakeDamagePatcher"/> class.</summary>
    internal MummyTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Mummy>(
            nameof(Mummy.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) });
    }

    #region harmony patches

    /// <summary>Crusader effect for intrinsic enchantments.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MummyTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.CurrentTool is MeleeWeapon && (who.CurrentTool as MeleeWeapon).hasEnchantmentOfType<CrusaderEnchantment>())
        // To: if (who.CurrentTool is MeleeWeapon weapon &&
        //        (weapon.hasEnchantmentOfType<CrusaderEnchantment>() || weapon.hasEnchantmentOfType<CursedEnchantment>() ||
        //         weapon.hasEnchantmentOfType<BlessedEnchantment>() || weapon.hasEnchantmentOfType<InfinityEnchantment>()))
        try
        {
            var makeJucier = generator.DefineLabel();
            var meleeWeapon = generator.DeclareLocal(typeof(MeleeWeapon));
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(CrusaderEnchantment))))
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S),
                    new CodeInstruction(OpCodes.Ldarg_S))
                .GetOperand(out var resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Stloc_S, meleeWeapon),
                    new CodeInstruction(OpCodes.Ldloc_S, meleeWeapon))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldarg_S))
                .RemoveInstructions(2) // redundant who.CurrentWeapon is MeleeWeapon
                .ReplaceInstructionWith(
                    new CodeInstruction(OpCodes.Ldloc_S, meleeWeapon)) // was OpCodes.Isinst typeof(MeleeWeapon)
                .AdvanceUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Brtrue_S, makeJucier)) // we are changing AND to OR
                .Advance() // beginning of Utility.makeTemporarySpriteJuicier( ... )
                .AddLabels(makeJucier)
                .InsertInstructions(
                    // or meleeWeapon.hasEnchantmentOfType<CursedEnchantment>()
                    new CodeInstruction(OpCodes.Ldloc_S, meleeWeapon),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(CursedEnchantment))),
                    new CodeInstruction(OpCodes.Brtrue_S, makeJucier),
                    // or meleeWeapon.hasEnchantmentOfType<BlessedEnchantment>()
                    new CodeInstruction(OpCodes.Ldloc_S, meleeWeapon),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(BlessedEnchantment))),
                    new CodeInstruction(OpCodes.Brtrue_S, makeJucier),
                    // or meleeWeapon.hasEnchantmentOfType<InfinityEnchantment>()
                    new CodeInstruction(OpCodes.Ldloc_S, meleeWeapon),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Tool).RequireMethod(nameof(Tool.hasEnchantmentOfType))
                            .MakeGenericMethod(typeof(InfinityEnchantment))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Cursed, Blessed and Infinity Crusader effects.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
