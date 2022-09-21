namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Common.Integrations.SpaceCore;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using CollectionExtensions = DaLion.Common.Extensions.Collections.CollectionExtensions;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuUpdatePatch"/> class.</summary>
    internal SkillLevelUpMenuUpdatePatch()
    {
        this.Target = "SpaceCore.Interface.SkillLevelUpMenu".ToType()
            .RequireMethod("update", new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void SkillLevelUpMenuUpdatePrefix(
        int ___currentLevel,
        bool ___hasUpdatedProfessions,
        ref bool ___informationUp,
        ref bool ___isActive,
        ref bool ___isProfessionChooser,
        List<int> ___professionsToChoose)
    {
        if (!___isProfessionChooser || !___hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            return;
        }

        ___isActive = false;
        ___informationUp = false;
        ___isProfessionChooser = false;
    }

    /// <summary>Patch to prevent duplicate profession acquisition.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: profPair = null; foreach ( ... )
        // To: profPair = ChooseProfessionPair(skill);
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldnull)) // find index of initializing profPair to null
                .ReplaceInstructionWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ChooseProfessionPair))))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        "SpaceCore.Interface.SkillLevelUpMenu"
                            .ToType()
                            .RequireField("currentSkill")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        "SpaceCore.Interface.SkillLevelUpMenu"
                            .ToType()
                            .RequireField("currentLevel")))
                .Advance(2)
                .RemoveInstructionsUntil(new CodeInstruction(OpCodes.Endfinally)); // remove the entire loop
        }
        catch (Exception ex)
        {
            Log.E(
                "Immersive Professions failed while patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                "\n—-- Do NOT report this to SpaceCore's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        // To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i]))
        var dontGetImmediatePerks = generator.DefineLabel();
        var i = 0;
        repeat:
        try
        {
            helper
                .FindNext(
                    // find index of adding a profession to the player's list of professions
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(NetList<int, NetInt>).RequireMethod("Add")))
                .Advance()
                .ReplaceInstructionWith(
                    // replace Add() with AddOrReplace()
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(CollectionExtensions)
                            .RequireMethod(nameof(CollectionExtensions.AddOrReplace))
                            .MakeGenericMethod(typeof(int))))
                .Advance()
                .InsertInstructions(
                    // skip adding perks if player already has them
                    new CodeInstruction(OpCodes.Brfalse_S, dontGetImmediatePerks))
                .AdvanceUntil(
                    // find isActive = false section
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld))
                .AddLabels(dontGetImmediatePerks); // branch here if the player already had the chosen profession
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching level up profession redundancy." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // repeat injection
        if (++i < 2)
        {
            goto repeat;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green;
        var skip = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu"
                        .ToType()
                        .RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu"
                        .ToType()
                        .RequireField("currentLevel")),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip))
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu"
                        .ToType()
                        .RequireField("professionsToChoose")),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, "SkillLevelUpMenu"
                        .ToType()
                        .RequireField("currentLevel")),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(SkillLevelUpMenuUpdatePatch).RequireMethod(nameof(ShouldSuppressClick))),
                    new CodeInstruction(OpCodes.Brtrue, skip))
                .FindNext(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4, 512))
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E("Immersive Professions failed while patching level up menu choice suppression." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static object? ChooseProfessionPair(object skillInstance, string skillId, int currentLevel)
    {
        if (currentLevel is not (5 or 10) || !CustomSkill.Loaded.TryGetValue(skillId, out var skill))
        {
            return null;
        }

        var professionPairs = ExtendedSpaceCoreApi.GetProfessionsForLevels.Value(skillInstance).Cast<object>().ToList();
        var levelFivePair = professionPairs[0];
        if (currentLevel == 5)
        {
            return levelFivePair;
        }

        var first = ExtendedSpaceCoreApi.GetFirstProfession.Value(levelFivePair);
        var second = ExtendedSpaceCoreApi.GetSecondProfession.Value(levelFivePair);
        var firstStringId = ExtendedSpaceCoreApi.GetProfessionStringId.Value(first);
        var secondStringId = ExtendedSpaceCoreApi.GetProfessionStringId.Value(second);
        var firstId = ModEntry.SpaceCoreApi!.GetProfessionId(skillId, firstStringId);
        var secondId = ModEntry.SpaceCoreApi.GetProfessionId(skillId, secondStringId);
        var branch = Game1.player.GetMostRecentProfession(firstId.Collect(secondId));
        return branch == firstId ? professionPairs[1] : professionPairs[2];
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return CustomProfession.LoadedProfessions.TryGetValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
