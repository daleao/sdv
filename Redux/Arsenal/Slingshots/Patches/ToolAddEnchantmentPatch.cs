namespace DaLion.Redux.Arsenal.Slingshots.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolAddEnchantmentPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ToolAddEnchantmentPatch"/> class.</summary>
    internal ToolAddEnchantmentPatch()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.AddEnchantment));
    }

    #region harmony patches

    /// <summary>Allow Slingshot forges.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ToolAddEnchantmentTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: if (this is MeleeWeapon ...
        // To: if (this is MeleeWeapon || this is Slingshot && ModEntry.SlingshotConfig.AllowForges ...
        try
        {
            var isWeapon = generator.DefineLabel();
            helper
                .FindFirst(new CodeInstruction(OpCodes.Isinst))
                .Advance()
                .GetOperand(out var resumeExecution)
                .InsertInstructions(
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
                        typeof(Arsenal.Config).RequirePropertyGetter(nameof(Arsenal.Config.Slingshots))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Config).RequirePropertyGetter(nameof(Config.AllowForges))))
                .Advance()
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
