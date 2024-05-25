namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationAnswerDialogueActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Transcendance.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(
        GameLocation __instance,
        ref bool __result,
        string? questionAndAnswer)
    {
        if ((!ShouldEnableSkillReset && !ShouldEnablePrestigeLevels && !ShouldEnableLimitBreaks) ||
            questionAndAnswer?.StartsWithAnyOf("dogStatue", "prestigeRespec", "skillReset") != true)
        {
            return true; // run original logic
        }

        if (questionAndAnswer.EndsWith("No"))
        {
            __result = false;
            return false; // don't run original logic
        }

        try
        {
            switch (questionAndAnswer)
            {
                case "dogStatue_Yes":
                {
                    OfferSkillResetChoices(__instance);
                    break;
                }

                case "dogStatue_prestigeRespec":
                {
                    OfferPrestigeRespecChoices(__instance);
                    break;
                }

                case "dogStatue_changeUlt":
                case "dogStatue_changeUlt_Return":
                {
                    OfferChangeLimitChoices(__instance);
                    break;
                }

                default:
                {
                    // if cancel do nothing
                    var skillNameAsSpan = questionAndAnswer.SplitWithoutAllocation('_')[1];
                    if (skillNameAsSpan.Equals("cancel", StringComparison.Ordinal) ||
                        skillNameAsSpan.Equals("Yes", StringComparison.Ordinal))
                    {
                        __result = true;
                        return false; // don't run original logic
                    }

                    // get skill type and do action
                    var skillName = skillNameAsSpan.ToString();
                    if (Skill.TryFromName(skillName, true, out var skill))
                    {
                        if (questionAndAnswer.Contains("skillReset_"))
                        {
                            HandleSkillReset(skill);
                        }
                        else if (questionAndAnswer.ContainsAnyOf("prestigeRespec_", "professionForget_"))
                        {
                            HandlePrestigeRespec(skill);
                        }
                    }
                    else if (CustomSkill.Loaded.TryGetValue(skillName, out var customSkill))
                    {
                        if (questionAndAnswer.Contains("skillReset_"))
                        {
                            HandleSkillReset(customSkill);
                        }
                        else if (questionAndAnswer.ContainsAnyOf("prestigeRespec_", "professionForget_"))
                        {
                            HandlePrestigeRespec(customSkill);
                        }
                    }

                    break;
                }
            }

            __result = true;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches

    #region dialog handlers

    private static void OfferSkillResetChoices(GameLocation location)
    {
        var skillResponses = (
            from skill in Skill.List.Concat<ISkill>(CustomSkill.Loaded.Values)
            where skill.CanReset()
            let costVal = skill.GetResetCost()
            let costStr = costVal > 0
                ? I18n.Prestige_DogStatue_Cost(costVal)
                : string.Empty
            select new Response(skill.StringId, skill.DisplayName + ' ' + costStr)).ToList();

        skillResponses.Add(new Response(
            "cancel",
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
        location.createQuestionDialogue(
            I18n.Prestige_DogStatue_Which(),
            skillResponses.ToArray(),
            "skillReset");
    }

    private static void OfferPrestigeRespecChoices(GameLocation location)
    {
        if (Config.Masteries.PrestigeRespecCost is var cost and > 0 && Game1.player.Money < cost)
        {
            Game1.drawObjectDialogue(
                Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var skillResponses = new List<Response>();
        if (GameLocation.canRespec(Skill.Farming))
        {
            skillResponses.Add(new Response(
                "farming",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));
        }

        if (GameLocation.canRespec(Skill.Fishing))
        {
            skillResponses.Add(new Response(
                "fishing",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));
        }

        if (GameLocation.canRespec(Skill.Foraging))
        {
            skillResponses.Add(new Response(
                "foraging",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));
        }

        if (GameLocation.canRespec(Skill.Mining))
        {
            skillResponses.Add(new Response(
                "mining",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));
        }

        if (GameLocation.canRespec(Skill.Combat))
        {
            skillResponses.Add(new Response(
                "combat",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608")));
        }

        foreach (var customSkill in CustomSkill.Loaded.Values)
        {
            if (customSkill.CurrentLevel >= 15 && !customSkill.NewLevels.Any(level => level is 15 or 20))
            {
                skillResponses.Add(new Response(
                    customSkill.StringId,
                    customSkill.DisplayName));
            }
        }

        skillResponses.Add(new Response(
            "cancel",
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
        location.createQuestionDialogue(
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueQuestion"),
            skillResponses.ToArray(),
            "prestigeRespec");
    }

    private static void OfferChangeLimitChoices(GameLocation location)
    {
        var player = Game1.player;
        if (Config.Masteries.LimitRespecCost is var cost and > 0 && player.Money < cost)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var choices = (
            from limit in LimitBreak.All()
            where player.HasProfession(limit.ParentProfession) && State.LimitBreak?.Equals(limit) != true
            select new Response(
                "Choice_" + limit.Name,
                I18n.Prestige_DogStatue_Choice(limit.ParentProfession.GetTitle(false), limit.DisplayName))).ToList();
        if (choices.Count == 0)
        {
            return;
        }

        if (State.LimitBreak is not null)
        {
            choices
                .Add(new Response("Cancel", I18n.Prestige_DogStatue_Cancel())
                .SetHotKey(Keys.Escape));
        }

        var message = State.LimitBreak is { } l
            ? I18n.Prestige_DogStatue_Replace(l.ParentProfession.GetTitle(false), l.DisplayName)
            : I18n.Prestige_DogStatue_Choose();
        location.createQuestionDialogue(message, [.. choices], HandleChangeLimit);
    }

    private static void HandleSkillReset(ISkill skill)
    {
        var player = Game1.player;
        var cost = skill.GetResetCost();
        if (cost > 0)
        {
            // check for funds and deduct cost
            if (player.Money < cost)
            {
                Game1.drawObjectDialogue(
                    Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                return;
            }

            player.Money = Math.Max(0, player.Money - cost);
        }

        // prepare to prestige at night
        State.SkillsToReset.Enqueue(skill);

        // play sound effect
        SoundBox.DogStatuePrestige.PlayLocal();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        State.UsedStatueToday = true;
    }

    private static void HandlePrestigeRespec(Skill skill)
    {
        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)Config.Masteries.PrestigeRespecCost);

        // remove all prestige professions for this skill
        for (var i = 0; i < 6; i++)
        {
            GameLocation.RemoveProfession(100 + (skill * 6) + i);
        }

        if (!ShouldEnableSkillReset)
        {
            for (var i = 0; i < 6; i++)
            {
                GameLocation.RemoveProfession((skill * 6) + i);
            }
        }

        var currentLevel = skill.CurrentLevel;

        // re-add prestige levels
        if (currentLevel >= 15)
        {
            player.newLevels.Add(new Point(skill, 15));
            if (!ShouldEnableSkillReset)
            {
                player.newLevels.Add(new Point(skill, 5));
            }
        }

        if (currentLevel >= 20)
        {
            player.newLevels.Add(new Point(skill, 20));
            if (!ShouldEnableSkillReset)
            {
                player.newLevels.Add(new Point(skill, 10));
            }
        }

        // play sound effect
        SoundBox.DogStatuePrestige.PlayLocal();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        State.UsedStatueToday = true;
    }

    private static void HandlePrestigeRespec(CustomSkill skill)
    {
        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)Config.Masteries.PrestigeRespecCost);

        // remove all prestige professions for this skill
        for (var i = 0; i < 6; i++)
        {
            GameLocation.RemoveProfession(100 + skill.Professions[i].Id);
        }

        var currentLevel = Farmer.checkForLevelGain(0, player.experiencePoints[0]);

        // re-add presige levels
        if (currentLevel >= 15)
        {
            Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
                .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 15));
        }

        if (currentLevel >= 20)
        {
            Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
                .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 20));
        }

        // play sound effect
        SoundBox.DogStatuePrestige.PlayLocal();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        State.UsedStatueToday = true;
    }

    private static void HandleChangeLimit(Farmer who, string choice)
    {
        if (choice == "Cancel")
        {
            return;
        }

        var split = choice.Split('_');
        if (split.Length != 2)
        {
            return;
        }

        var newLimit = LimitBreak.FromName(split[1]);
        var message = '"' + newLimit.Description + "\". " + I18n.Prestige_DogStatue_Describe();
        Response[] responses =
        [
            new Response("Confirm_" + newLimit.Name, Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes"))
                .SetHotKey(Keys.Y),
            new Response("Return", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
                .SetHotKey(Keys.Escape),
        ];

        who.currentLocation.createQuestionDialogue(message, responses, "ConfirmChangeLimit");
    }

    #endregion dialog handlers
}
