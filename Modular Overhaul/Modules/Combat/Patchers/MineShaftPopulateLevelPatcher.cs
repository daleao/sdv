namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftPopulateLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftPopulateLevelPatcher"/> class.</summary>
    internal MineShaftPopulateLevelPatcher()
    {
        this.Target = this.RequireMethod<MineShaft>("populateLevel");
    }

    #region harmony patches

    /// <summary>Spawn some more containers.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MineShaftPopulateLevelTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var dontRebalance = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_5) })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_5) })
                .AddLabels(dontRebalance)
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
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnableWeaponOverhaul))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontRebalance),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 17),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed to increasing container spawn (base).\nHelper returned {ex}");
            return null;
        }

        try
        {
            var dontRebalance = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R8, 20.0) })
                .AddLabels(dontRebalance)
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
                            typeof(Config).RequirePropertyGetter(nameof(Config.EnableWeaponOverhaul))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontRebalance), new CodeInstruction(OpCodes.Ldc_R8, 80.0),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed to increasing container spawn (luck).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
