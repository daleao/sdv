namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.Configs;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolAddEnchantmentPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolAddEnchantmentPatcher"/> class.</summary>
    internal ToolAddEnchantmentPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.AddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolAddEnchantmentTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (this is MeleeWeapon ...
        // To: if (this is MeleeWeapon || this is Slingshot && ArsenalModule.Config.Slingshots.EnableForges ...
        try
        {
            var isWeapon = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Isinst) })
                .Move()
                .GetOperand(out var resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brtrue_S, isWeapon),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Isinst, typeof(Slingshot)),
                        new CodeInstruction(OpCodes.Brfalse, resumeExecution),
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.Slingshots))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(SlingshotConfig).RequirePropertyGetter(nameof(SlingshotConfig.EnableForges))),
                    })
                .Move()
                .AddLabels(isWeapon);
        }
        catch (Exception ex)
        {
            Log.E($"Failed allowing add forges to slingshots.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
