namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Configs;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using SpaceCore.Interface;
using SCPair = SpaceCore.Skills.Skill.ProfessionPair;
using SCSkill = SpaceCore.Skills.Skill;

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
    private static bool LevelUpMenuUpdatePrefix(
        SkillLevelUpMenu __instance,
        List<int> ___professionsToChoose,
        List<TemporaryAnimatedSprite> ___littleStars,
        List<CraftingRecipe> ___newCraftingRecipes,
        ref int ___currentLevel,
        ref string ___currentSkill,
        ref bool ___isProfessionChooser,
        ref bool ___hasUpdatedProfessions,
        ref int ___timerBeforeStart,
        ref string ___title,
        ref List<string> ___extraInfoForLevel,
        ref List<string> ___leftProfessionDescription,
        ref MouseState ___oldMouseState,
        ref SCPair? ___profPair,
        GameTime time)
    {
        if (__instance.isProfessionChooser && __instance.hasUpdatedProfessions && ___professionsToChoose.Count == 2 &&
            ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) &&
            ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            __instance.isActive = false;
            __instance.informationUp = false;
            __instance.isProfessionChooser = false;
            return true; // run original logic
        }

        if (!__instance.isActive || ProfessionsModule.Config.Prestige.Mode !=
            PrestigeConfig.PrestigeMode.Streamlined || ___currentLevel is not 15 or 20)
        {
            return true; // run original logic
        }

        #region copy

        var xPositionOnScreen = __instance.xPositionOnScreen;
        var width = __instance.width;
        var skillsByName = Reflector
            .GetStaticFieldGetter<Dictionary<string, SCSkill>>(typeof(SpaceCore.Skills), "SkillsByName")
            .Invoke();
        if (!___hasUpdatedProfessions)
        {
            var scSkill = skillsByName[___currentSkill];
            ___profPair = ChooseProfessionPair(___currentSkill, ___currentLevel);
            if (___profPair is not null)
            {
                ___isProfessionChooser = true;
            }
            else
            {
                return true;
            }

            var currentBranch = Game1.player.GetCurrentBranchForSkill(CustomSkill.FromSpaceCore(scSkill)!);
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

            var scProfession = ___profPair.First.GetVanillaId() == currentBranch ? ___profPair.First : ___profPair.Second;
            var la = new List<string>(new[] { scProfession.GetName() });
            la.AddRange(scProfession.GetDescription().Split('\n'));
            ___leftProfessionDescription = la;
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
                    0f)
                { local = true, });
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
            var newLevel = Reflector
                .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                .Invoke();
            ___currentLevel = newLevel.First().Value;
            ___currentSkill = newLevel.First().Key;
            ___title = Game1.content.LoadString(
                "Strings\\UI:LevelUp_Title",
                ___currentLevel,
                skillsByName[___currentSkill].GetName());
            ___extraInfoForLevel = __instance.getExtraInfoForLevel(___currentSkill, ___currentLevel);
            ___profPair = ChooseProfessionPair(___currentSkill, ___currentLevel);
            if (___profPair is not null)
            {
                ___professionsToChoose.Clear();
                ___isProfessionChooser = true;
                var currentBranch = Game1.player.GetCurrentBranchForSkill(Skill.FromValue(___currentLevel));
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
            }

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
            if (Game1.didPlayerJustLeftClick() && __instance.readyToClose())
            {
                __instance.okButtonClicked();
                var scSkill = skillsByName[___currentSkill];
                var currentBranch = Game1.player.GetCurrentBranchForSkill(CustomSkill.FromSpaceCore(scSkill)!);
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
                    _ =>
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
                    _ =>
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
            Log.E("Professions module failed patching level up profession redundancy." +
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
                            typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(CongratulateForUnlockingPrestigeLevels))),
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
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static SCPair? ChooseProfessionPair(string skillId, int currentLevel)
    {
        if (currentLevel % 5 != 0 ||
            !CustomSkill.Loaded.TryGetValueAs<string, ISkill, CustomSkill>(skillId, out var customSkill))
        {
            return null;
        }

        var skillInstance = customSkill.ToSpaceCore()!;
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

    private static bool HasUnlockedPrestigeLevels(int currentLevel, string skillId)
    {
        if (!ProfessionsModule.EnablePrestigeLevels || currentLevel != 10 ||
            !CustomSkill.Loaded.TryGetValueAs<string, ISkill, CustomSkill>(skillId, out var customSkill))
        {
            return false;
        }

        switch (ProfessionsModule.Config.Prestige.Mode)
        {
            case PrestigeConfig.PrestigeMode.Streamlined:
                return true;

            case PrestigeConfig.PrestigeMode.Standard:
            {
                var hasAllProfessions = Game1.player.HasAllProfessionsInSkill(customSkill);
                Log.D($"[Prestige]: Farmer {Game1.player.Name} " + (hasAllProfessions
                ? $" has acquired all professions in the {customSkill} skill and may now gain extended levels."
                : $" does not yet have all professions in the {customSkill} skill."));
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

    private static void CongratulateForUnlockingPrestigeLevels(string skillId)
    {
        switch (ProfessionsModule.Config.Prestige.Mode)
        {
            case PrestigeConfig.PrestigeMode.Standard:
                Game1.drawObjectDialogue(I18n.Prestige_LevelUp_Unlocked_Standard(CustomSkill.Loaded[skillId].DisplayName));
                break;
            case PrestigeConfig.PrestigeMode.Challenge:
                Game1.drawObjectDialogue(I18n.Prestige_LevelUp_Unlocked_Challenge());
                break;
        }
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return CustomProfession.Loaded.TryGetValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected subroutines
}
