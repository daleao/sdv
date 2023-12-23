namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Configs;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuUpdatePatcher"/> class.</summary>
    internal LevelUpMenuUpdatePatcher()
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), new[] { typeof(GameTime) });
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuUpdatePrefix(
        LevelUpMenu __instance,
        List<int> ___professionsToChoose,
        List<TemporaryAnimatedSprite> ___littleStars,
        List<CraftingRecipe> ___newCraftingRecipes,
        ref int ___currentLevel,
        ref int ___currentSkill,
        ref bool ___isProfessionChooser,
        ref bool ___hasUpdatedProfessions,
        ref int ___timerBeforeStart,
        ref string ___title,
        ref List<string> ___extraInfoForLevel,
        ref List<string> ___leftProfessionDescription,
        ref MouseState ___oldMouseState,
        ref Rectangle ___sourceRectForLevelIcon,
        GameTime time)
    {
        if (!__instance.isProfessionChooser)
        {
            return true; // run original logic
        }

        if (__instance.hasUpdatedProfessions && ___professionsToChoose.Count == 2 &&
            ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) &&
            ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            __instance.isActive = false;
            __instance.informationUp = false;
            __instance.isProfessionChooser = false;
            return true; // run original logic
        }

        if (!__instance.isActive ||
            ProfessionsModule.Config.Prestige.Mode != PrestigeConfig.PrestigeMode.Streamlined ||
            ___currentLevel is not 15 or 20 || ___currentSkill == 5)
        {
            return true; // run original logic
        }

        #region copy

        var xPositionOnScreen = __instance.xPositionOnScreen;
        var width = __instance.width;
        if (!___hasUpdatedProfessions)
        {
            var currentBranch = Game1.player.GetCurrentBranchForSkill(Skill.FromValue(___currentSkill));
            switch (___currentLevel)
            {
                case 15:
                    ___professionsToChoose.Add(currentBranch);
                    break;
                case 20:
                    var currentLeaf =
                        Game1.player.GetCurrentLeafProfessionForBranch(Profession.FromValue(currentBranch));
                    ___professionsToChoose.Add(currentLeaf);
                    break;
            }

            ___leftProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[0]);
            ___hasUpdatedProfessions = true;
        }

        for (var i = ___littleStars.Count - 1; i >= 0; i--)
        {
            if (___littleStars[i].update(time))
            {
                ___littleStars.RemoveAt(i);
            }
        }

        if (Game1.random.NextDouble() < 0.03)
        {
            var position =
                new Vector2(
                    0f,
                    (Game1.random.Next(__instance.yPositionOnScreen - 128, __instance.yPositionOnScreen - 4) / 20 * 4 *
                     5) + 32)
                {
                    X = Game1.random.NextBool()
                        ? Game1.random.Next(
                            xPositionOnScreen + (width / 2) - 228,
                            xPositionOnScreen + (width / 2) - 132)
                        : Game1.random.Next(
                            xPositionOnScreen + (width / 2) + 116,
                            xPositionOnScreen + width - 160),
                };

            if (position.Y < __instance.yPositionOnScreen - 64 - 8)
            {
                position.X = Game1.random.Next(
                    xPositionOnScreen + (width / 2) - 116,
                    xPositionOnScreen + (width / 2) + 116);
            }

            position.X = position.X / 20f * 4f * 5f;
            ___littleStars.Add(
                new TemporaryAnimatedSprite(
                    "LooseSprites\\Cursors",
                    new Rectangle(364, 79, 5, 5),
                    80f,
                    7,
                    1,
                    position,
                    flicker: false,
                    flipped: false,
                    1f,
                    0f,
                    Color.White,
                    4f,
                    0f,
                    0f,
                    0f) { local = true, });
        }

        if (___timerBeforeStart > 0)
        {
            ___timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
            if (___timerBeforeStart > 0 || !Game1.options.SnappyMenus)
            {
                return false;
            }

            __instance.populateClickableComponentList();
            __instance.snapToDefaultClickableComponent();

            return false;
        }

        __instance.height = 512;
        ___oldMouseState = Game1.input.GetMouseState();
        if (__instance.isActive && !__instance.informationUp && __instance.starIcon != null)
        {
            __instance.starIcon.sourceRect.X =
                __instance.starIcon.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 294 : 310;
        }

        if (__instance.isActive && __instance.starIcon != null && !__instance.informationUp &&
            (___oldMouseState.LeftButton == ButtonState.Pressed ||
             (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A))) &&
            __instance.starIcon.containsPoint(___oldMouseState.X, ___oldMouseState.Y))
        {
            ___newCraftingRecipes.Clear();
            ___extraInfoForLevel.Clear();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.playSound("bigSelect");
            __instance.informationUp = true;
            __instance.isProfessionChooser = false;
            var newLevel = Game1.player.newLevels[0];
            ___currentLevel = newLevel.Y;
            ___currentSkill = newLevel.X;
            ___title = Game1.content.LoadString(
                "Strings\\UI:LevelUp_Title",
                ___currentLevel,
                Farmer.getSkillDisplayNameFromIndex(___currentSkill));
            ___extraInfoForLevel = __instance.getExtraInfoForLevel(___currentSkill, ___currentLevel);
            ___sourceRectForLevelIcon = ___currentSkill switch
            {
                0 => new Rectangle(0, 0, 16, 16),
                1 => new Rectangle(16, 0, 16, 16),
                3 => new Rectangle(32, 0, 16, 16),
                2 => new Rectangle(80, 0, 16, 16),
                4 => new Rectangle(128, 16, 16, 16),
                5 => new Rectangle(64, 0, 16, 16),
                _ => ___sourceRectForLevelIcon,
            };

            ___professionsToChoose.Clear();
            ___isProfessionChooser = true;
            var currentBranch = Game1.player.GetCurrentBranchForSkill(Skill.FromValue(___currentSkill));
            switch (___currentLevel)
            {
                case 15:
                    ___professionsToChoose.Add(currentBranch);
                    break;
                case 20:
                    var currentLeaf = Game1.player.GetCurrentLeafProfessionForBranch(Profession.FromValue(currentBranch));
                    ___professionsToChoose.Add(
                        Game1.player.GetCurrentLeafProfessionForBranch(Profession.FromValue(currentLeaf)));
                    break;
            }

            ___leftProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[0]);

            var newHeight = 0;
            __instance.height = newHeight + 256 + (___extraInfoForLevel.Count * 64 * 3 / 4);
            Game1.player.freezePause = 100;
        }

        if (!__instance.isActive || !__instance.informationUp)
        {
            return false;
        }

        Game1.player.completelyStopAnimatingOrDoingAction();
        if (__instance.okButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
        {
            __instance.okButton.scale = Math.Min(1.1f, __instance.okButton.scale + 0.05f);
            if ((___oldMouseState.LeftButton == ButtonState.Pressed ||
                 (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A))) &&
                __instance.readyToClose())
            {
                __instance.okButtonClicked();
                var currentBranch = Game1.player.GetCurrentBranchForSkill(Skill.FromValue(___currentSkill));
                switch (___currentLevel)
                {
                    case 15:
                        if (Game1.player.professions.AddOrReplace(currentBranch + 100))
                        {
                            __instance.getImmediateProfessionPerk(currentBranch + 100);
                        }

                        break;
                    case 20:
                        var currentLeaf =
                            Game1.player.GetCurrentLeafProfessionForBranch(Profession.FromValue(currentBranch));
                        ___professionsToChoose.Add(currentLeaf);
                        if (Game1.player.professions.AddOrReplace(currentLeaf + 100))
                        {
                            __instance.getImmediateProfessionPerk(currentLeaf + 100);
                        }

                        break;
                }

                __instance.RemoveLevelFromLevelList();
            }
        }
        else
        {
            __instance.okButton.scale = Math.Max(1f, __instance.okButton.scale - 0.05f);
        }

        Game1.player.freezePause = 100;

        #endregion copy

        return false; // don't run original logic
    }

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (currentLevel == 5)
        // To: if (currentLevel is 5 or 15)
        for (var i = 0; i < 2; i++)
        {
            try
            {
                var isLevel5 = generator.DefineLabel();
                helper
                    .Match(
                        new[]
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                            new CodeInstruction(OpCodes.Ldc_I4_5), new CodeInstruction(OpCodes.Bne_Un_S),
                        })
                    .Match(new[] { new CodeInstruction(OpCodes.Bne_Un_S) })
                    .Insert(
                        new[]
                        {
                            new CodeInstruction(OpCodes.Beq_S, isLevel5), new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                            new CodeInstruction(OpCodes.Ldc_I4_S, 15),
                        })
                    .Move()
                    .AddLabels(isLevel5);
            }
            catch (Exception ex)
            {
                Log.E($"Failed patching level 15 profession choices.\nHelper returned {ex}");
                return null;
            }
        }

        helper.GoTo(0);

        // This injection chooses the correct 2nd-tier profession choices based on the last selected level 5 profession.
        // From: else if (Game1.player.professions.Contains(currentSkill * 6))
        // To: else if (GetCurrentBranchForSkill(currentSkill) == currentSkill * 6)
        for (var i = 0; i < 2; i++)
        {
            try
            {
                helper
                    .Match(
                        new[]
                        {
                            // find index of checking if the player has the the first level 5 profession in the skill
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                            new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                            new CodeInstruction(OpCodes.Ldc_I4_6), new CodeInstruction(OpCodes.Mul),
                            new CodeInstruction(
                                OpCodes.Callvirt,
                                typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                        },
                        ILHelper.SearchOption.First)
                    .GetLabels(out var labels)
                    .Remove(2) // remove loading the local player's professions
                    .AddLabels(labels)
                    .Move(2)
                    .Insert(
                        new[]
                        {
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(GetCurrentBranchForSkill))),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                        })
                    .Match(
                        new[]
                        {
                            new CodeInstruction(
                                OpCodes.Callvirt,
                                typeof(NetList<int, NetInt>).RequireMethod(nameof(NetList<int, NetInt>.Contains))),
                        })
                    .Remove() // remove Callvirt Nelist<int, NetInt>.Contains()
                    .SetOpCode(OpCodes.Bne_Un_S); // was Brfalse_S
            }
            catch (Exception ex)
            {
                Log.E(
                    $"Failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession.\nHelper returned {ex}");
                return null;
            }
        }

        helper.GoTo(0);

        var chosenProfession = generator.DeclareLocal(typeof(int));
        var shouldProposeFinalQuestion = generator.DeclareLocal(typeof(bool));
        var shouldCongratulateFullSkillMastery = generator.DeclareLocal(typeof(bool));

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        //		  getImmediateProfessionPerk(professionsToChoose[i]);
        // To: if (!Game1.player.professions.AddOrReplace(professionsToChoose[i])) getImmediateProfessionPerk(professionsToChoose[i]);
        //     if (currentLevel > 10) Game1.player.professions.Add(100 + professionsToChoose[i]);
        //		- and also -
        // Injected: if (ShouldProposeFinalQuestion(professionsToChoose[i])) shouldProposeFinalQuestion = true;
        //			  if (ShouldCongratulateOnFullPrestige(currentLevel, professionsToChoose[i])) shouldCongratulateOnFullPrestige = true;
        // Before: isActive = false;
        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var dontGetImmediatePerks = generator.DefineLabel();
                        var isNotPrestigeLevel = generator.DefineLabel();
                        helper
                            .Match(
                                new[]
                                {
                                    // find index of adding a profession to the player's list of professions
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(List<int>).RequirePropertyGetter("Item")),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(NetList<int, NetInt>).RequireMethod("Add")),
                                })
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
                                        typeof(LevelUpMenu).RequireField("currentLevel")),
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
                                    // load the chosen profession onto the stack
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    // check if should propose final question
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(LevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(ShouldProposeChangeUltimate))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
                                    // load the current level onto the stack
                                    new CodeInstruction(OpCodes.Ldarg_0),
                                    new CodeInstruction(
                                        OpCodes.Ldfld,
                                        typeof(LevelUpMenu).RequireField("currentLevel")),
                                    // load the chosen profession onto the stack
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                                    // check if should congratulate on full prestige
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(LevelUpMenuUpdatePatcher).RequireMethod(
                                            nameof(HasUnlockedPrestigeLevels))),
                                    // store the bool result for later
                                    new CodeInstruction(OpCodes.Stloc_S, shouldCongratulateFullSkillMastery),
                                },
                                // branch here if was not prestige level
                                new[] { isNotPrestigeLevel });
                    });
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed patching level up profession redundancy and injecting dialogues.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (shouldProposeFinalQuestion) ProposeFinalQuestion(chosenProfession)
        // And: if (shouldCongratulateOnFullPrestige) CongratulateOnFullPrestige(chosenProfession)
        // Before: if (!isActive || !informationUp)
        try
        {
            var dontProposeFinalQuestion = generator.DefineLabel();
            var dontCongratulateOnFullPrestige = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .GoTo(0)
                .Insert(
                    new[]
                    {
                        // initialize shouldProposeFinalQuestion local variable to false
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Stloc_S, shouldProposeFinalQuestion),
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
                            typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isActive))),
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
                        // check if should propose the final question
                        new CodeInstruction(OpCodes.Ldloc_S, shouldProposeFinalQuestion),
                        new CodeInstruction(OpCodes.Brfalse_S, dontProposeFinalQuestion),
                        // if so, push the chosen profession onto the stack and call ProposeFinalQuestion()
                        new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ProposeChangeUltimate))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    // restore backed-up labels
                    labels)
                .Insert(
                    new[]
                    {
                        // check if should congratulate on full prestige
                        new CodeInstruction(OpCodes.Ldloc_S, shouldCongratulateFullSkillMastery),
                        new CodeInstruction(OpCodes.Brfalse_S, dontCongratulateOnFullPrestige),
                        // if so, push the chosen profession onto the stack and call CongratulateOnFullPrestige()
                        new CodeInstruction(OpCodes.Ldloc_S, chosenProfession),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(CongratulateForUnlockingPrestigeLevels))),
                    },
                    // branch here after checking for proposal
                    new[] { dontProposeFinalQuestion });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting level up profession final question.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green; (x2)
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
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
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("professionsToChoose")),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Callvirt, typeof(List<int>).RequirePropertyGetter("Item")),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(ShouldSuppressClick))),
                        new CodeInstruction(OpCodes.Brtrue, skip),
                    })
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldc_I4, 512),
                    })
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching level up menu choice suppression.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool ShouldProposeChangeUltimate(int chosenProfession)
    {
        return ProfessionsModule.Config.Limit.EnableLimitBreaks && ProfessionsModule.EnableSkillReset &&
               chosenProfession.IsIn(26..29) && Game1.player.Get_Ultimate() is not null &&
               Game1.player.Get_Ultimate()!.Value != chosenProfession;
    }

    private static bool HasUnlockedPrestigeLevels(int currentLevel, int chosenProfession)
    {
        if (!ProfessionsModule.EnablePrestigeLevels || currentLevel != 10 ||
            !Skill.TryFromValue(chosenProfession / 6, out var skill) || skill == Farmer.luckSkill)
        {
            return false;
        }

        switch (ProfessionsModule.Config.Prestige.Mode)
        {
            case PrestigeConfig.PrestigeMode.Streamlined:
                return true;

            case PrestigeConfig.PrestigeMode.Standard:
            {
                var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(skill);
                Log.D($"[Prestige]: Farmer {Game1.player.Name} " + (hasAllProfessions
                    ? $" has acquired all professions in the {skill} skill and may now gain extended levels."
                    : $" does not yet have all professions in the {skill} skill."));
                return hasAllProfessions;
            }

            case PrestigeConfig.PrestigeMode.Challenge:
            {
                var hasAllProfessions = Game1.player.HasAllProfessions(true);
                Log.D($"[Prestige]: Farmer {Game1.player.Name} " + (hasAllProfessions
                    ? " has acquired all professions and may now gain extended levels."
                    : " does not yet have all professions."));
                return hasAllProfessions;
            }

            default:
                return false;
        }
    }

    private static void ProposeChangeUltimate(int chosenProfession, bool shouldCongratulateFullSkillMastery)
    {
        var ulti = Game1.player.Get_Ultimate()!;
        var oldProfession = Profession.FromValue(ulti);
        var newProfession = Profession.FromValue(chosenProfession);
        Game1.currentLocation.createQuestionDialogue(
            I18n.Prestige_LevelUp_Question(
                    oldProfession.Title,
                    Ultimate.FromValue(oldProfession).DisplayName,
                    newProfession.Title,
                    Ultimate.FromValue(newProfession).DisplayName),
            Game1.currentLocation.createYesNoResponses(),
            (_, answer) =>
            {
                if (answer == "Yes")
                {
                    Game1.player.Set_Ultimate(Ultimate.FromValue(chosenProfession));
                }

                if (shouldCongratulateFullSkillMastery)
                {
                    CongratulateForUnlockingPrestigeLevels(chosenProfession);
                }
            });
    }

    private static void CongratulateForUnlockingPrestigeLevels(int chosenProfession)
    {
        switch (ProfessionsModule.Config.Prestige.Mode)
        {
            case PrestigeConfig.PrestigeMode.Standard:
                Game1.drawObjectDialogue(I18n.Prestige_LevelUp_Unlocked_Standard(Skill.FromValue(chosenProfession / 6).DisplayName));
                break;
            case PrestigeConfig.PrestigeMode.Challenge:
                Game1.drawObjectDialogue(I18n.Prestige_LevelUp_Unlocked_Challenge());
                break;
        }
    }

    private static int GetCurrentBranchForSkill(int currentSkill)
    {
        return Game1.player.GetCurrentBranchForSkill(Skill.FromValue(currentSkill));
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return Profession.TryFromValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
