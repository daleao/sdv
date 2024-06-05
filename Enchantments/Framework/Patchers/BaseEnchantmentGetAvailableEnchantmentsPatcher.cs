namespace DaLion.Enchantments.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseEnchantmentGetAvailableEnchantmentsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BaseEnchantmentGetAvailableEnchantmentsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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
            helper
                .GoTo(4)
                .Remove(12)
                .Insert(
                [
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
                    new CodeInstruction(OpCodes.Newobj, typeof(EnergizedMeleeEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add explosive enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(ExplosiveEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add mammon enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(MammoniteEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    //// add steadfast enchant
                    //new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    //new CodeInstruction(OpCodes.Newobj, typeof(SteadfastEnchantment).RequireConstructor()),
                    //new CodeInstruction(
                    //    OpCodes.Callvirt,
                    //    typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add vampiric enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(VampiricEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add wabbajack enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(WabbajackEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add stabbing enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(StabbingEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add freezing enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(ChillingEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add quincy enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(QuincyEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add runaan enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(RunaanEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add ranged energized enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(EnergizedSlingshotEnchantment).RequireConstructor()),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                ]);
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
