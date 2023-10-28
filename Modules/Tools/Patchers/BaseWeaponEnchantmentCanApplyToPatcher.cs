namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Tools.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class BaseWeaponEnchantmentCanApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BaseWeaponEnchantmentCanApplyToPatcher"/> class.</summary>
    internal BaseWeaponEnchantmentCanApplyToPatcher()
    {
        this.Target = this.RequireMethod<BaseWeaponEnchantment>("CanApplyTo");
    }

    #region harmony patches

    /// <summary>Allow Haymaker Scythe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? BaseWeaponEnchantmentCanApplyToTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var isNotScythe = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(MeleeWeapon).RequireMethod(nameof(MeleeWeapon.isScythe))),
                    })
                .Move()
                .GetOperand(out var cannotApply)
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotScythe))
                .Move()
                .AddLabels(isNotScythe)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(HaymakerEnchantment)),
                        new CodeInstruction(OpCodes.Brfalse_S, (Label)cannotApply),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tools))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ToolConfig).RequirePropertyGetter(nameof(ToolConfig.Scythe))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ScytheConfig).RequirePropertyGetter(nameof(ScytheConfig.AllowHaymakerEnchantment))),
                        new CodeInstruction(OpCodes.Brfalse_S, (Label)cannotApply),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing Haymaker enchantment on Scythe.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
