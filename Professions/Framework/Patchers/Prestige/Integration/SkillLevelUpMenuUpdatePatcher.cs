namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
using CollectionExtensions = DaLion.Shared.Extensions.Collections.CollectionExtensions;
using SCPair = SCSkill.ProfessionPair;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillLevelUpMenuUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillLevelUpMenuUpdatePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SkillLevelUpMenuUpdatePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SkillLevelUpMenu>(nameof(SkillLevelUpMenu.update), [typeof(GameTime)]);
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool SkillLevelUpMenuUpdatePrefix(
        SkillLevelUpMenu __instance,
        List<int> ___professionsToChoose,
        List<TemporaryAnimatedSprite> ___littleStars,
        List<CraftingRecipe> ___newCraftingRecipes,
        int ___currentLevel,
        string ___currentSkill,
        ref bool ___isProfessionChooser,
        ref bool ___hasUpdatedProfessions,
        ref int ___timerBeforeStart,
        ref string ___title,
        ref List<string> ___extraInfoForLevel,
        ref List<string> ___leftProfessionDescription,
        ref List<string> ___rightProfessionDescription,
        ref MouseState ___oldMouseState,
        ref SCPair? ___profPair,
        GameTime time)
    {
        if (!___isProfessionChooser)
        {
            return true; // run original logic
        }

        if (___hasUpdatedProfessions && ___professionsToChoose.Count == 2 &&
            ShouldSuppressClick(___professionsToChoose[0], ___currentLevel) &&
            ShouldSuppressClick(___professionsToChoose[1], ___currentLevel))
        {
            __instance.isActive = false;
            __instance.informationUp = false;
            __instance.isProfessionChooser = false;
            return true; // run original logic
        }

        if (!__instance.isActive || ___currentLevel is 15 or 20)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            var skillsByName = Reflector
                .GetStaticFieldGetter<Dictionary<string, SCSkill>>(typeof(SCSkills), "SkillsByName")
                .Invoke();
            if (!___hasUpdatedProfessions)
            {
                ___profPair = ChooseProfessionPair(___currentSkill, ___currentLevel);
                if (___profPair is not null)
                {
                    ___isProfessionChooser = true;
                    ___professionsToChoose.Add(___profPair.First.GetVanillaId());
                    ___professionsToChoose.Add(___profPair.Second.GetVanillaId());
                }
                else
                {
                    return true; // run original logic
                }

                if (___professionsToChoose.Count == 0)
                {
                    return true; // run original logic
                }

                var scProfession = ___profPair.First.GetVanillaId() == ___professionsToChoose[0]
                    ? ___profPair.First
                    : ___profPair.Second;
                var la = new List<string>([scProfession.GetName()]);
                la.AddRange(scProfession.GetDescription().Split('\n'));
                ___leftProfessionDescription = la;
                if (___professionsToChoose.Count > 1)
                {
                    scProfession = ___profPair.Second.GetVanillaId() == ___professionsToChoose[1]
                        ? ___profPair.Second
                        : ___profPair.First;
                    var ra = new List<string>([scProfession.GetName()]);
                    ra.AddRange(scProfession.GetDescription().Split('\n'));
                    ___rightProfessionDescription = ra;
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
                var newLevel = Reflector
                    .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
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
                    if (player.professions.Contains(___profPair.First.GetVanillaId()))
                    {
                        ___professionsToChoose.Add(___profPair.First.GetVanillaId());
                    }

                    if (player.professions.Contains(___profPair.Second.GetVanillaId()))
                    {
                        ___professionsToChoose.Add(___profPair.Second.GetVanillaId());
                    }
                }

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
                if (Game1.didPlayerJustLeftClick() && __instance.readyToClose())
                {
                    __instance.okButtonClicked();
                    var scSkill = skillsByName[___currentSkill];
                    var root = player.GetCurrentRootProfessionForSkill(CustomSkill.GetFromSpaceCore(scSkill)!);
                    switch (___currentLevel)
                    {
                        case 15:
                            if (player.professions.AddOrReplace(root + 100))
                            {
                                __instance.getImmediateProfessionPerk(root + 100);
                            }

                            break;
                        case 20:
                            var branch =
                                player.GetCurrentBranchingProfessionForRoot(Profession.FromValue(root));
                            ___professionsToChoose.Add(branch);
                            if (player.professions.AddOrReplace(branch + 100))
                            {
                                __instance.getImmediateProfessionPerk(branch + 100);
                            }

                            break;
                    }
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

    /// <summary>Patch to prevent duplicate profession acquisition.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
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
                    [
                        // find index of initializing profPair to null
                        new CodeInstruction(OpCodes.Ldnull),
                        new CodeInstruction(OpCodes.Stfld, typeof(SkillLevelUpMenu).RequireField("profPair")),
                    ],
                    _ => helper
                        .ReplaceWith(
                            new CodeInstruction(
                                OpCodes.Call,
                                typeof(SkillLevelUpMenuUpdatePatcher).RequireMethod(nameof(ChooseProfessionPair))))
                        .Insert([
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(
                                OpCodes.Ldfld,
                                typeof(SkillLevelUpMenu).RequireField("currentSkill")),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(
                                OpCodes.Ldfld,
                                typeof(SkillLevelUpMenu).RequireField("currentLevel")),
                        ])
                        .Move(2)
                        .CountUntil([new CodeInstruction(OpCodes.Endfinally)], out var count)
                        .Remove(count));
        }
        catch (Exception ex)
        {
            Log.E(
                "Professions mod failed patching 2nd-tier profession choices to reflect last chosen 1st-tier profession." +
                $"\nHelper returned {ex}");
            return null;
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
                                    typeof(SkillLevelUpMenu).RequireField("currentLevel")),
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
        // Before: leftProfessionColor = Color.Green;
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
                ])
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Green))),
                ])
                .Insert([
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
                ])
                .PatternMatch(
                    [new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldc_I4, 512),])
                .AddLabels(skip);
        }
        catch (Exception ex)
        {
            Log.E("Professions mod failed patching level up menu choice suppression." +
                  $"\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static SCPair? ChooseProfessionPair(string skillId, int currentLevel)
    {
        if (currentLevel % 5 != 0 || !CustomSkill.Loaded.TryGetValue(skillId, out var customSkill))
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

    private static bool ShouldSuppressClick(int hovered, int currentLevel)
    {
        return CustomProfession.Loaded.TryGetValue(hovered, out var profession) &&
               ((currentLevel == 5 && Game1.player.HasAllProfessionsBranchingFrom(profession)) ||
                (currentLevel == 10 && Game1.player.HasProfession(profession)));
    }

    #endregion injected
}
