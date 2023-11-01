namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.ContentPatcher;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.ContentPatcher", "Content Patcher", "1.30.0")]
internal sealed class SkillLevelValueProviderSkillLevelsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelValueProviderSkillLevelsPatcher"/> class.</summary>
    internal SkillLevelValueProviderSkillLevelsPatcher()
    {
        this.Target = "ContentPatcher.Framework.Tokens.ValueProviders.SkillLevelValueProvider"
            .ToType().RequireMethod("<UpdateContext>b__4_0");
    }

    #region harmony patches

    /// <summary>Patch to cap Content Patcher's skill level value provider to level 10.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelValueProviderSkillLevelsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CombatLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.FarmingLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.FishingLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.ForagingLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.LuckLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.MiningLevel))),
                    })
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed correcting Content Patcher's skill level conditions.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
