namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Weapons.Enchantments;
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
        var helper = new IlHelper(original, instructions);

        try
        {
            var newWeaponEnchants = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(4)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Arsenal.Config).RequirePropertyGetter(nameof(Arsenal.Config.Weapons))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.LigoEnchants))),
                    new CodeInstruction(OpCodes.Brtrue_S, newWeaponEnchants))
                .Advance(12)
                .InsertInstructions(new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .InsertWithLabels(
                    new[] { newWeaponEnchants },
                    // add redux artful enchant
                    new CodeInstruction(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new CodeInstruction(OpCodes.Newobj, typeof(ReduxArtfulEnchantment).RequireConstructor()),
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
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))))
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting new weapon enchants.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
