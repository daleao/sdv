namespace DaLion.Overhaul.Modules.Enchantments.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Enchantments.Ranged;
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
            var newMeleeEnchantments = generator.DefineLabel();
            var newRangedEnchantments = generator.DefineLabel();
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
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.MeleeEnchantments))),
                        new CodeInstruction(OpCodes.Brtrue_S, newMeleeEnchantments),
                    })
                .Move(12)
                .Insert(new[] { new CodeInstruction(OpCodes.Br_S, newRangedEnchantments) })
                .Insert(
                    new[]
                    {
                        // add redux artful enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(NewArtfulEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
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
                        new CodeInstruction(OpCodes.Newobj, typeof(TributeEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add bloodthirsty enchant
                        new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new CodeInstruction(OpCodes.Newobj, typeof(BloodthirstyEnchantment).RequireConstructor()),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    },
                    new[] { newMeleeEnchantments })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Enchantments))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.RangedEnchantments))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),

                        // add engorging enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(EngorgingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add gatling enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(GatlingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add preserving enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(PreservingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add quincy enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(QuincyEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                        // add spreading enchant
                        new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                        new(OpCodes.Newobj, typeof(SpreadingEnchantment).RequireConstructor()), new(
                            OpCodes.Callvirt,
                            typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    },
                    new[] { newRangedEnchantments })
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
