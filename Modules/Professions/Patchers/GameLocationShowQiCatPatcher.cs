namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationShowQiCatPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationShowQiCatPatcher"/> class.</summary>
    internal GameLocationShowQiCatPatcher()
    {
        this.Target = typeof(Utility).GetInnerMethodsContaining("<ShowQiCat>b__303_3").SingleOrDefault();
    }

    #region harmony patches

    /// <summary>Patch to display new perfection requirement.</summary>
    [HarmonyPrefix]
    // ReSharper disable once UnusedParameter.Local
    private static bool UtilityPerceGameLocationShowQiCatPrefix(ref float __result)
    {
        if (!ProfessionsModule.Config.EnablePrestige || !ProfessionsModule.Config.ExtendedPerfectionRequirement)
        {
            return true; // run original logic
        }

        if (ProfessionsModule.Config.EnableExtendedProgression)
        {
            // ReSharper disable once RedundantAssignment
            __result = Math.Min(Skill.List.Where(skill => skill.CurrentLevel >= skill.MaxLevel).Sum(_ => 1f), 5f);
        }
        else
        {
            // ReSharper disable once RedundantAssignment
            __result += Math.Min(Skill.List.Where(skill => Game1.player.HasAllProfessionsInSkill(skill)).Sum(_ => 1f), 5f);
        }

        return false; // don't run original logic
    }

    /// <summary>Patch to add perfection requirement with Extended Level.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? UtilityPercentGameCompleteTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var vanilla = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldstr, "/25^") })
                .AddLabels(vanilla)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionConfig).RequirePropertyGetter(nameof(ProfessionConfig.EnablePrestige))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanilla), new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionConfig).RequirePropertyGetter(
                                nameof(ProfessionConfig.ExtendedPerfectionRequirement))),
                        new CodeInstruction(OpCodes.Brfalse_S, vanilla),
                        new CodeInstruction(OpCodes.Ldstr, "/5^"),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla level requirement.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
