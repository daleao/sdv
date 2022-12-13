namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Sounds;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatcher"/> class.</summary>
    internal GameLocationAnswerDialogueActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, ref bool __result, string? questionAndAnswer)
    {
        if (questionAndAnswer is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (!ProfessionsModule.Config.EnablePrestige ||
            ((!questionAndAnswer.Contains("dogStatue") || questionAndAnswer.Contains("No")) &&
             !questionAndAnswer.ContainsAnyOf("prestigeRespec_", "skillReset_")))
        {
            return true; // run original logic
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
                {
                    OfferChangeUltiChoices(__instance);
                    break;
                }

                default:
                {
                    // if cancel do nothing
                    var skillName = questionAndAnswer.Split('_')[1];
                    if (skillName is "cancel" or "Yes")
                    {
                        __result = true;
                        return false; // don't run original logic
                    }

                    // get skill type and do action
                    if (Skill.TryFromName(skillName, true, out var skill))
                    {
                        if (questionAndAnswer.Contains("skillReset_"))
                        {
                            HandleSkillReset(skill);
                        }
                        else if (questionAndAnswer.Contains("prestigeRespec_"))
                        {
                            HandlePrestigeRespec(skill);
                        }
                    }
                    else if (SCSkill.Loaded.TryGetValue(skillName, out var customSkill))
                    {
                        if (questionAndAnswer.Contains("skillReset_"))
                        {
                            HandleSkillReset(skill);
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
            from skill in Skill.List.Except(Skill.Luck.Collect()).Concat(SCSkill.Loaded.Values)
            where skill.CanReset()
            let costVal = skill.GetResetCost()
            let costStr = costVal > 0
                ? I18n.Get("prestige.dogstatue.cost", new { cost = costVal })
                : string.Empty
            select new Response(skill.StringId, skill.DisplayName + ' ' + costStr)).ToList();

        skillResponses.Add(new Response(
            "cancel",
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
        location.createQuestionDialogue(
            I18n.Get("prestige.dogstatue.which"),
            skillResponses.ToArray(),
            "skillReset");
    }

    private static void OfferPrestigeRespecChoices(GameLocation location)
    {
        if (ProfessionsModule.Config.PrestigeRespecCost > 0 && Game1.player.Money < ProfessionsModule.Config.PrestigeRespecCost)
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

        skillResponses.Add(new Response(
            "cancel",
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
        location.createQuestionDialogue(
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueQuestion"),
            skillResponses.ToArray(),
            "prestigeRespec");
    }

    private static void OfferChangeUltiChoices(GameLocation location)
    {
        if (ProfessionsModule.Config.ChangeUltCost > 0 && Game1.player.Money < ProfessionsModule.Config.ChangeUltCost)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var chosenUltimate = Game1.player.Get_Ultimate()!;
        var pronoun = chosenUltimate.GetBuffPronoun();

        string message = I18n.Get(
            "prestige.dogstatue.replace",
            new { pronoun, profession = chosenUltimate.Profession.Title, ultimate = chosenUltimate.DisplayName });

        var choices = (
            from unchosenUltimate in Game1.player.GetUnchosenUltimates()
            orderby unchosenUltimate
            let choice = I18n.Get(
                "prestige.dogstatue.choice",
                new { profession = unchosenUltimate.Profession.Title, ultimate = unchosenUltimate.DisplayName })
            select new Response("Choice_" + unchosenUltimate, choice)).ToList();

        choices.Add(new Response("Cancel", I18n.Get("prestige.dogstatue.cancel"))
            .SetHotKey(Keys.Escape));

        location.createQuestionDialogue(message, choices.ToArray(), HandleChangeUlti);
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
        player.Get_SkillsToReset().Enqueue(skill);

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        player.Set_UsedStatueToday(true);
    }

    private static void HandlePrestigeRespec(Skill skill)
    {
        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)ProfessionsModule.Config.PrestigeRespecCost);

        // remove all prestige professions for this skill
        Enumerable.Range(100 + (skill * 6), 6).ForEach(GameLocation.RemoveProfession);

        var currentLevel = Farmer.checkForLevelGain(0, player.experiencePoints[0]);
        if (currentLevel >= 15)
        {
            player.newLevels.Add(new Point(skill, 15));
        }

        if (currentLevel >= 20)
        {
            player.newLevels.Add(new Point(skill, 20));
        }

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        player.Set_UsedStatueToday(true);
    }

    private static void HandleChangeUlti(Farmer who, string choice)
    {
        if (choice == "Cancel")
        {
            return;
        }

        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)ProfessionsModule.Config.ChangeUltCost);

        // change ultimate
        var chosenUltimate = Ultimate.FromName(choice.Split("_")[1]);
        player.Set_Ultimate(chosenUltimate);

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        string pronoun = I18n.Get("article.indefinite" + (Game1.player.IsMale ? ".male" : ".female"));
        Game1.drawObjectDialogue(I18n.Get(
            "prestige.dogstatue.fledged",
            new { pronoun, profession = chosenUltimate.Profession.Title }));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        who.Set_UsedStatueToday(true);
    }

    #endregion dialog handlers
}
