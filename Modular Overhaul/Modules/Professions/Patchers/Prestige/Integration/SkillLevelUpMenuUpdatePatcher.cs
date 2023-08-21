namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuUpdatePatcher"/> class.</summary>
    internal SkillLevelUpMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<SkillLevelUpMenu>(nameof(SkillLevelUpMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static void LevelUpMenuUpdatePrefix(
        SkillLevelUpMenu __instance, int ___currentLevel, List<int> ___professionsToChoose)
    {
        if (!__instance.isProfessionChooser || !__instance.hasUpdatedProfessions ||
            !ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) ||
            !ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            return;
        }

        __instance.isActive = false;
        __instance.informationUp = false;
        __instance.isProfessionChooser = false;
        __instance.hasUpdatedProfessions = true;
    }

    /// <summary>Patch to prevent duplicate profession acquisition.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillLevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: profPair = null; foreach ( ... )
        // To: profPair = ChooseProfessionPair(skill);
        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        // find index of initializing profPair to null
                        new CodeInstruction(OpCodes.Ldnull),
                        new CodeInstruction(OpCodes.Stfld, typeof(SkillLevelUpMenu).RequireField("profPair")),
                    },
                    () =>
                    {
                        helper
                            .ReplaceWith(
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ChooseProfessionPair))))
                            .Insert(
                                new[]
                                {
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                                })
                            .Move(2)
                            .CountUntil(new[] { new CodeInstruction(OpCodes.Endfinally) }, out var count)
                            .Remove(count); // remove the entire loop
                    });
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions module failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                "\n—-- Do NOT report this to SpaceCore's author. ---" +
                $"\nHelper returned {ex}");
            return null;
        }

        var chosenProfession = generator.DeclareLocal(typeof(int));
        var shouldCongratulateFullSkillMastery = generator.DeclareLocal(typeof(bool));

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        //		  getImmediateProfessionPerk(professionsToChoose[i]);
        // To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i])) getImmediateProfessionPerk(professionsToChoose[i]);
        //     if (currentLevel > 10) Game1.player.professions.Add(100 + professionsToChoose[i]);
        //		- and also -
        // Injected: if (ShouldCongratulateOnFullPrestige(currentLevel, professionsToChoose[i])) shouldCongratulateOnFullPrestige = true;
        // Before: isActive = false;
        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        // find index of adding a profession to the player's list of professions
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetList<int, NetInt>).RequireMethod("Add")),
                    },
                    () =>
                    {
                        var dontGetImmediatePerks = generator.DefineLabel();
                        var isNotPrestigeLevel = generator.DefineLabel();
                        helper
                            .Move()
                            .Insert(
                                new[]
                                {
                                    // duplicate chosen profession
                                    new CodeInstruction(OpCodes.Dup),
                                    // store it for later
                                    new CodeInstruction(OpCodes.Stloc_S, chosenProfession),
                                })
                            .ReplaceWith(
                                // replace Add() with AddOrReplace()
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(Shared.Extensions.Collections.CollectionExtensions)
                                        .RequireMethod(
                                            nameof(Shared.Extensions.Collections.CollectionExtensions
                                                .AddOrReplace))
                                        .MakeGenericMethod(typeof(int))))
                            .Move()
                            .Insert(
                                new[]
                                {
                                    // skip adding perks if player already has them
                                    new CodeInstruction(OpCodes.Brfalse_S, dontGetImmediatePerks),
                                })
                            .Match(
                                new[]
                                {
                                    // find isActive = false section
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldc_I4_0),
                                    new CodeInstruction(OpCodes.Stfld),
                                })
                            .Insert(
                                new[]
                                {
                                    // check if current level is above 10 (i.e. prestige level)
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                                    new CodeInstruction(OpCodes.Ble_Un_S, isNotPrestigeLevel), // branch out if not
                                    // add chosenProfession + 100 to player's professions
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(Farmer).RequireField(nameof(Farmer.professions))),
                                    new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    new CodeInstruction(OpCodes.Add),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Add))),
                                },
                                // branch here if the player already had the chosen profession
                                new[] { dontGetImmediatePerks })
                            .Insert(
                                new[]
                                {
                                    // load the current level onto the stack
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                                    // load the current skill id
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                                    // check if should congratulate on full prestige
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(HasAcquiredLastProfession))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                                },
                                // branch here if was not prestige level
                                new[] { isNotPrestigeLevel });
                    });
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up profession redundancy." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        // Injected: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
        // Before: if (!isActive || !informationUp)
        try
        {
            var dontCongratulateOnFullPrestige = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(0)
                .Insert(
                    new[]
                    {
                        // initialize shouldCongratulateOnFullPrestige local variable to false
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                    })
                .Match(
                    new[]
                    {
                        // find index of the section that checks for a return (once LevelUpMenu is no longer needed)
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField(nameof(SkillLevelUpMenu.isActive))),
                    },
                    ILHelper.SearchOption.Last)
                .Move(-1) // retreat to the start of this section
                .StripLabels(out var labels) // backup and remove branch labels
                .AddLabels(
                    dontCongratulateOnFullPrestige,
                    resumeExecution) // branch here after checking for congratulate or after proposing final question
                .Insert(
                    new[]
                    {
                        // check if should congratulate on full prestige
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                        // if so, push the current skill id onto the stack and call CongratulateOnFullPrestige()
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(CongratulateForAcquiringLastProfession))),
                    },
                    // restore backed-up labels
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting level up profession final question.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green;
        try
        {
            var skip = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                    },
                    ILHelper.SearchOption.First)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(SkillLevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldc_I4, 512), })
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E("Professions module failed patching level up menu choice suppression." +
                  "\n—-- Do NOT report this to SpaceCore's author. ---" +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static object? ChooseProfessionPair(string skillId, int currentLevel)
    {
        if (currentLevel % 5 != 0 || !SCSkill.Loaded.TryGetValue(skillId, out var iSkill))
        {
            return null;
        }

        var scSkill = (SCSkill)iSkill;
        var skillInstance = scSkill.ToSpaceCore()!;
        var professionPairs = skillInstance.ProfessionsForLevels.ToList();
        var levelFivePair = professionPairs[0];
        if (currentLevel is 5 or 15)
        {
            return levelFivePair;
        }

        var firstId = levelFivePair.First.GetVanillaId();
        var secondId = levelFivePair.Second.GetVanillaId();
        var branch = Game1.player.GetMostRecentProfession(firstId.Collect(secondId));
        return branch == firstId ? professionPairs[1] : professionPairs[2];
    }

    private static bool HasAcquiredLastProfession(int currentLevel, string skillId)
    {
        if (!ProfessionsModule.Config.EnablePrestige || currentLevel != 10 ||
            !SCSkill.Loaded.TryGetValue(skillId, out var scSkill))
        {
            return false;
        }

        var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(scSkill);
        Log.D($"[Prestige]: Farmer {Game1.player.Name} " + (hasAllProfessions
            ? $" has acquired all professions in the {scSkill} skill and may now gain extended levels."
            : $" does not yet have all professions in the {scSkill} skill."));
        if (hasAllProfessions)
        {
            return true;
        }

#if DEBUG
        var missing = string.Join(
            ',',
            Game1.player
                .GetMissingProfessionsInSkill(scSkill)
                .Select(p => p.Title));
        Log.D($"[Prestige]: Missing professions: {missing}.");
#endif
        return false;
    }

    private static void CongratulateForAcquiringLastProfession(string skillId)
    {
        if (ProfessionsModule.Config.EnableExtendedProgession)
        {
            Game1.drawObjectDialogue(I18n.Prestige_Levelup_Unlocked(SCSkill.Loaded[skillId].DisplayName));
        }

        if (!Game1.player.HasAllProfessions(true))
        {
            return;
        }

        string title = _I18n.Get("prestige.achievement.title" + (Game1.player.IsMale ? ".male" : ".female"));
        if (!Game1.player.achievements.Contains(title.GetDeterministicHashCode()))
        {
            EventManager.Enable<AchievementUnlockedDayStartedEvent>();
        }
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return SCProfession.Loaded.TryGetValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
