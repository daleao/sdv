namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Configs;
using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetAvailableEnchantmentsPatcher"/> class.</summary>
    internal BaseEnchantmentGetAvailableEnchantmentsPatcher()
    {
        this.Target = this.RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
    }

    #region harmony patches

    /// <summary>Out with the old and in with the new enchants.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseEnchantmentGetAvailableEnchantmentsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var newEnchantments = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(4)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.RingsEnchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(RingsEnchantmentsConfig).RequirePropertyGetter(nameof(RingsEnchantmentsConfig.NewPrismaticEnchantments))),
                        new CodeInstruction(OpCodes.Brtrue_S, newEnchantments),
                    })
                .Move(12)
                .Insert(new[] { new CodeInstruction(OpCodes.Br_S, resumeExecution) })
                .Insert(
                    new[]
                    {
                        // add carving enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(CarvingEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add cleaving enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(CleavingEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add energized enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(EnergizedEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add explosive enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(ExplosiveEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add tribute enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(MammoniteEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add bloodthirsty enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(BloodthirstyEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add steadfast enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(SteadfastEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add wabbajack enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(WabbajackEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add freezing enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(FreezingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add quincy enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(QuincyEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add runaan enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(RunaanEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add ranged energized enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(RangedEnergizedEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    },
                    new[] { newEnchantments })
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting new enchants.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
