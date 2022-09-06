namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using Enchantments;
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseEnchantmentGetAvailableEnchantmentsPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BaseEnchantmentGetAvailableEnchantmentsPatch()
    {
        Target = RequireMethod<BaseEnchantment>(nameof(BaseEnchantment.GetAvailableEnchantments));
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
            var newWeaponEnchants = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(4)
                .InsertInstructions(
                    new(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new(OpCodes.Call, typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.NewWeaponEnchants))),
                    new(OpCodes.Brtrue_S, newWeaponEnchants)
                )
                .Advance(12)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] { newWeaponEnchants },
                    // add cleaving enchant
                    new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new(OpCodes.Newobj, typeof(CleavingEnchantment).RequireConstructor()),
                    new(OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add energized enchant
                    new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new(OpCodes.Newobj, typeof(EnergizedEnchantment).RequireConstructor()),
                    new(OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add tribute enchant
                    new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new(OpCodes.Newobj, typeof(TributeEnchantment).RequireConstructor()),
                    new(OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add reworked vampiric enchant
                    new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new(OpCodes.Newobj, typeof(VampiricEnchantment).RequireConstructor()),
                    new(OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add))),
                    // add magic / sunburst enchant
                    new(OpCodes.Ldsfld, typeof(BaseEnchantment).RequireField("_enchantments")),
                    new(OpCodes.Newobj, typeof(MagicEnchantment).RequireConstructor()),
                    new(OpCodes.Callvirt,
                        typeof(List<BaseEnchantment>).RequireMethod(nameof(List<BaseEnchantment>.Add)))
                );
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