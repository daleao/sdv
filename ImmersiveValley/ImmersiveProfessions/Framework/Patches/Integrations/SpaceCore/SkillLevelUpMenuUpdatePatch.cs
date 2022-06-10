namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Stardew.Professions.Integrations;
using Extensions;

using CollectionExtensions = DaLion.Common.Extensions.Collections.CollectionExtensions;

#endregion using directives

[UsedImplicitly]
internal class SkillLevelUpMenuPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillLevelUpMenuPatch()
    {
        try
        {
            Original = "SpaceCore.Interface.SkillLevelUpMenu".ToType().RequireMethod("update", new[] {typeof(GameTime)});
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuUpdateTranspiler(
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
                        typeof(SkillLevelUpMenuPatch).RequireMethod(nameof(ChooseProfessionPair)))
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
            Log.E($"Failed while patching 2nd-tier profession choices to reflect last chosen 1st-tier profession. Helper returned {ex}");
            transpilationFailed = true;
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
            Log.E($"Failed while patching level up profession redundancy. Helper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        // repeat injection
        if (++i < 2) goto repeat;

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [CanBeNull]
    private static object ChooseProfessionPair(object skillInstance, string skillId, int currentLevel)
    {
        if (currentLevel is not (5 or 10)) return null;

        var professionPairs = ((IEnumerable) SpaceCoreIntegration.GetProfessionsForLevels.Invoke(skillInstance, null)!)
            .Cast<object>().ToList();
        var levelFivePair = professionPairs[0];
        if (currentLevel == 5) return levelFivePair;

        var first = SpaceCoreIntegration.GetFirstProfession.Invoke(levelFivePair, null)!;
        var second = SpaceCoreIntegration.GetSecondProfession.Invoke(levelFivePair, null)!;
        var firstStringId = (string) SpaceCoreIntegration.GetProfessionStringId.Invoke(first, null)!;
        var secondStringId = (string) SpaceCoreIntegration.GetProfessionStringId.Invoke(second, null)!;
        var firstId = ModEntry.SpaceCoreApi!.GetProfessionId(skillId, firstStringId);
        var secondId = ModEntry.SpaceCoreApi!.GetProfessionId(skillId, secondStringId);
        var branch = Game1.player.GetMostRecentProfession(firstId.Collect(secondId));
        return branch == firstId ? professionPairs[1] : professionPairs[2];
    }

    #endregion injected subroutines
}