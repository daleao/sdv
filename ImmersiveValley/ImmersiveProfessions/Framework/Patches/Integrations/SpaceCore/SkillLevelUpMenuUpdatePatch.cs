namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.SpaceCore;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CollectionExtensions = DaLion.Common.Extensions.Collections.CollectionExtensions;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillLevelUpMenuUpdatePatch()
    {
        Target = "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireMethod("update", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void SkillLevelUpMenuUpdatePrefix(int ___currentLevel, bool ___hasUpdatedProfessions,
        ref bool ___informationUp, ref bool ___isActive, ref bool ___isProfessionChooser,
        List<int> ___professionsToChoose)
    {
        if (!___isProfessionChooser || !___hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel)) return;

        ___isActive = false;
        ___informationUp = false;
        ___isProfessionChooser = false;
    }

    /// <summary>Patch to prevent duplicate profession acquisition.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        /// From: profPair = null; foreach ( ... )
        /// To: profPair = ChooseProfessionPair(skill);

        try
        {
            helper
                .FindFirst( // find index of initializing profPair to null
                    new CodeInstruction(OpCodes.Ldnull)
                )
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ChooseProfessionPair)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireField("currentSkill")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireField("currentLevel"))
                )
                .Advance(2)
                .RemoveUntil( // remove the entire loop
                    new CodeInstruction(OpCodes.Endfinally)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching 2nd-tier profession choices to reflect last chosen 1st-tier profession.\nHelper returned {ex}");
            return null;
        }

        /// From: Game1.player.professions.Add(professionsToChoose[i]);
        /// To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i]))

        var dontGetImmediatePerks = generator.DefineLabel();
        var i = 0;
    repeat:
        try
        {
            helper
                .FindNext( // find index of adding a profession to the player's list of professions
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).RequireMethod("Add"))
                )
                .Advance()
                .ReplaceWith( // replace Add() with AddOrReplace()
                    new(OpCodes.Call,
                        typeof(CollectionExtensions)
                            .RequireMethod(
                                nameof(CollectionExtensions.AddOrReplace))
                            .MakeGenericMethod(typeof(int)))
                )
                .Advance()
                .Insert(
                    // skip adding perks if player already has them
                    new CodeInstruction(OpCodes.Brfalse_S, dontGetImmediatePerks)
                )
                .AdvanceUntil( // find isActive = false section
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld)
                )
                .AddLabels(dontGetImmediatePerks); // branch here if the player already had the chosen profession
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up profession redundancy.\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat;

        /// Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        /// Before: leftProfessionColor = Color.Green;

        var skip = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu".ToType().RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu".ToType().RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Call, typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu".ToType().RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu".ToType().RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Call, typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip)
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4, 512)
                )
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up menu choice suppression.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static object? ChooseProfessionPair(object skillInstance, string skillId, int currentLevel)
    {
        if (currentLevel is not (5 or 10) || !ModEntry.CustomSkills.TryGetValue(skillId, out var skill)) return null;

        var professionPairs = ExtendedSpaceCoreAPI.GetProfessionsForLevels.Value(skillInstance).Cast<object>().ToList();
        var levelFivePair = professionPairs[0];
        if (currentLevel == 5) return levelFivePair;

        var first = ExtendedSpaceCoreAPI.GetFirstProfession.Value(levelFivePair);
        var second = ExtendedSpaceCoreAPI.GetSecondProfession.Value(levelFivePair);
        var firstStringId = ExtendedSpaceCoreAPI.GetProfessionStringId.Value(first);
        var secondStringId = ExtendedSpaceCoreAPI.GetProfessionStringId.Value(second);
        var firstId = ModEntry.SpaceCoreApi!.GetProfessionId(skillId, firstStringId);
        var secondId = ModEntry.SpaceCoreApi.GetProfessionId(skillId, secondStringId);
        var branch = Game1.player.GetMostRecentProfession(firstId.Collect(secondId));
        return branch == firstId ? professionPairs[1] : professionPairs[2];
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel) =>
        ModEntry.CustomProfessions.TryGetValue(hovered, out var profession) &&
               (currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession) ||
               currentLevel == 10 && Game1.player.HasProfession(profession));

    #endregion injected subroutines
}