namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Menus;
using CollectionExtensions = DaLion.Shared.Extensions.Collections.CollectionExtensions;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal LevelUpMenuUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.update), [typeof(GameTime)]);
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuUpdatePrefix(
        LevelUpMenu __instance,
        List<int> ___professionsToChoose,
        TemporaryAnimatedSpriteList ___littleStars,
        List<CraftingRecipe> ___newCraftingRecipes,
        int ___currentLevel,
        int ___currentSkill,
        ref bool ___isProfessionChooser,
        ref bool ___hasUpdatedProfessions,
        ref int ___timerBeforeStart,
        ref string ___title,
        ref List<string> ___extraInfoForLevel,
        ref List<string> ___leftProfessionDescription,
        ref List<string> ___rightProfessionDescription,
        ref MouseState ___oldMouseState,
        ref Rectangle ___sourceRectForLevelIcon,
        GameTime time)
    {
        if (___currentSkill >= Farmer.luckSkill || !__instance.isProfessionChooser)
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

        if (!__instance.isActive || ___currentLevel <= 10)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            if (!___hasUpdatedProfessions)
            {
                ISkill skill = Skill.FromValue(___currentSkill);
                switch (___currentLevel)
                {
                    case 15:
                        ___professionsToChoose.AddRange(skill.TierOneProfessionIds.Where(player.professions.Contains));
                        break;
                    case 20:
                        var rootId = player.GetCurrentRootProfessionForSkill(skill);
                        IProfession root = Profession.FromValue(rootId);
                        ___professionsToChoose.AddRange(root.GetBranchingProfessions
                            .Select(p => p.Id)
                            .Where(player.professions.Contains));
                        break;
                }

                if (___professionsToChoose.Count == 0)
                {
                    return true; // run original logic
                }

                ___leftProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[0]);
                if (___professionsToChoose.Count > 1)
                {
                    ___rightProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[1]);
                }

                ___hasUpdatedProfessions = true;
            }

            if (___professionsToChoose.Count != 1)
            {
                return true; // run original logic
            }

            #region choose single profession

            for (var i = ___littleStars.Count - 1; i >= 0; i--)
            {
                if (___littleStars[i].update(time))
                {
                    ___littleStars.RemoveAt(i);
                }
            }

            var xPositionOnScreen = __instance.xPositionOnScreen;
            var width = __instance.width;
            if (Game1.random.NextBool(0.03))
            {
                var position =
                    new Vector2(
                        0f,
                        // ReSharper disable once PossibleLossOfFraction
                        (Game1.random.Next(__instance.yPositionOnScreen - 128, __instance.yPositionOnScreen - 4) / 20 *
                         4 *
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
            if (__instance is { isActive: true, informationUp: false, starIcon: not null })
            {
                __instance.starIcon.sourceRect.X =
                    __instance.starIcon.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? 294 : 310;
            }

            if (__instance is { isActive: true, starIcon: not null, informationUp: false } &&
                (___oldMouseState.LeftButton == ButtonState.Pressed ||
                 (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A))) &&
                __instance.starIcon.containsPoint(___oldMouseState.X, ___oldMouseState.Y))
            {
                ___newCraftingRecipes.Clear();
                ___extraInfoForLevel.Clear();
                player.completelyStopAnimatingOrDoingAction();
                Game1.playSound("bigSelect");
                __instance.informationUp = true;
                __instance.isProfessionChooser = false;
                (___currentSkill, ___currentLevel) = player.newLevels[0];
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
                ISkill skill = Skill.FromValue(___currentSkill);
                switch (___currentLevel)
                {
                    case 15:
                        ___professionsToChoose.AddRange(skill.TierOneProfessionIds.Where(player.professions.Contains));
                        break;
                    case 20:
                        var rootId = player.GetCurrentRootProfessionForSkill(skill);
                        IProfession root = Profession.FromValue(rootId);
                        ___professionsToChoose.AddRange(root.GetBranchingProfessions
                            .Select(p => p.Id)
                            .Where(player.professions.Contains));
                        break;
                }

                ___leftProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[0]);
                if (___professionsToChoose.Count > 1)
                {
                    ___rightProfessionDescription = LevelUpMenu.getProfessionDescription(___professionsToChoose[1]);
                }

                const int newHeight = 0;
                __instance.height = newHeight + 256 + (___extraInfoForLevel.Count * 64 * 3 / 4);
                player.freezePause = 100;
            }

            if (!__instance.isActive || !__instance.informationUp)
            {
                return false;
            }

            player.completelyStopAnimatingOrDoingAction();
            if (__instance.okButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                __instance.okButton.scale = Math.Min(1.1f, __instance.okButton.scale + 0.05f);
                if ((___oldMouseState.LeftButton == ButtonState.Pressed ||
                     (Game1.options.gamepadControls && Game1.oldPadState.IsButtonDown(Buttons.A))) &&
                    __instance.readyToClose())
                {
                    __instance.okButtonClicked();
                    var rootId = player.GetCurrentRootProfessionForSkill(Skill.FromValue(___currentSkill));
                    switch (___currentLevel)
                    {
                        case 15:
                            if (player.professions.AddOrReplace(rootId + 100))
                            {
                                __instance.getImmediateProfessionPerk(rootId + 100);
                            }

                            break;
                        case 20:
                            var branch =
                                player.GetCurrentBranchingProfessionForRoot(Profession.FromValue(rootId));
                            ___professionsToChoose.Add(branch);
                            if (player.professions.AddOrReplace(branch + 100))
                            {
                                __instance.getImmediateProfessionPerk(branch + 100);
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

            player.freezePause = 100;

            #endregion choose single profession

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    /// <summary>Patch to prevent duplicate profession acquisition + display end of level up dialogues.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
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
                    .PatternMatch([
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldc_I4_5), new CodeInstruction(OpCodes.Bne_Un_S),
                    ])
                    .PatternMatch([new CodeInstruction(OpCodes.Bne_Un_S)])
                    .Insert([
                        new CodeInstruction(OpCodes.Beq_S, isLevel5), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 15),
                    ])
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
                    .PatternMatch(
                        [
                            // find index of checking if the player has the first level 5 profession in the skill
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                            new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.professions))),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                            new CodeInstruction(OpCodes.Ldc_I4_6), new CodeInstruction(OpCodes.Mul),
                            new CodeInstruction(
                                OpCodes.Callvirt,
                                typeof(NetIntHashSet).RequireMethod(nameof(NetIntHashSet.Contains))),
                        ],
                        ILHelper.SearchOption.First)
                    .GetLabels(out var labels)
                    .Remove(2) // remove loading the local player's professions
                    .AddLabels(labels)
                    .Move(2)
                    .Insert([
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LevelUpMenuUpdatePatcher).RequireMethod(nameof(GetCurrentBranchForSkill))),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentSkill")),
                    ])
                    .PatternMatch([
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NetIntHashSet).RequireMethod(nameof(NetIntHashSet.Contains))),
                    ])
                    .Remove() // remove Callvirt NetIntHashSet.Contains()
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
        var chosenProfessionAdjusted = generator.DeclareLocal(typeof(int));

        // From: Game1.player.professions.Add(professionsToChoose[i]);
        // To: Game1.player.professions.Add(temp = currentLevel > 10 ? professionsToChoose[i] + 100 : professionsToChoose[i]);
        // + State.OrderedProfessions.AddOrReplace(temp)

        try
        {
            helper
                .Repeat(
                    2,
                    _ =>
                    {
                        var isNotPrestigeLevel = generator.DefineLabel();
                        helper
                            .PatternMatch([
                                // find index of adding a profession to the player's set of professions
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(List<int>).RequirePropertyGetter("Item")),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(NetHashSet<int>).RequireMethod("Add")),
                            ])
                            .Move()
                            .Insert([
                                // duplicate chosen profession
                                new CodeInstruction(OpCodes.Dup),
                                // store it for later
                                new CodeInstruction(OpCodes.Stloc_S, chosenProfession),
                                new CodeInstruction(OpCodes.Ldarg_0),
                                new CodeInstruction(
                                    OpCodes.Ldfld,
                                    typeof(LevelUpMenu).RequireField("currentLevel")),
                                new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                                new CodeInstruction(OpCodes.Ble_Un_S, isNotPrestigeLevel), // branch out if not
                                new CodeInstruction(OpCodes.Ldc_I4_S, 100),
                                new CodeInstruction(OpCodes.Add),
                            ])
                            .Insert(
                                [
                                    // mirror to ordered professions
                                    new CodeInstruction(OpCodes.Dup),
                                    new CodeInstruction(OpCodes.Stloc_S, chosenProfessionAdjusted),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(State))),
                                    new CodeInstruction(
                                        OpCodes.Callvirt,
                                        typeof(ProfessionsState).RequirePropertyGetter(
                                            nameof(State.OrderedProfessions))),
                                    new CodeInstruction(OpCodes.Ldloc_S, chosenProfessionAdjusted),
                                    new CodeInstruction(
                                        OpCodes.Call,
                                        typeof(CollectionExtensions).RequireMethod(
                                            nameof(CollectionExtensions.AddOrReplace)).MakeGenericMethod(typeof(int))),
                                    new CodeInstruction(OpCodes.Pop),
                                ],
                                [isNotPrestigeLevel]);
                    });
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed patching level up menu prestige acquisition.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (!ShouldSuppressClick(chosenProfession[i], currentLevel))
        // Before: leftProfessionColor = Color.Green; (x2)
        try
        {
            var skip = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                    ],
                    ILHelper.SearchOption.First)
                .Insert([
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
                ])
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                ])
                .Insert([
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
                ])
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4, 512),
                ])
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

    #region injections

    private static int GetCurrentBranchForSkill(int currentSkill)
    {
        if (currentSkill < Farmer.luckSkill)
        {
            return Game1.player.GetCurrentRootProfessionForSkill(Skill.FromValue(currentSkill));
        }

        var branch = currentSkill * 6;
        return Game1.player.professions.Contains(branch) ? branch : branch + 1;
    }

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return Profession.TryFromValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injections
}
