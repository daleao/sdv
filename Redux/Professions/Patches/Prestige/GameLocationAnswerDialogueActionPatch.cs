namespace DaLion.Redux.Professions.Patches.Prestige;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Redux.Professions.Events.GameLoop;
using DaLion.Redux.Professions.Extensions;
using DaLion.Redux.Professions.Sounds;
using DaLion.Redux.Professions.Ultimates;
using DaLion.Redux.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialogueActionPatch"/> class.</summary>
    internal GameLocationAnswerDialogueActionPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, string questionAndAnswer)
    {
        if (!ModEntry.Config.Professions.EnablePrestige ||
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
                            HandlePrestigeRespect(skill);
                        }
                    }
                    else if (CustomSkill.Loaded.TryGetValue(skillName, out var customSkill))
                    {
                        if (questionAndAnswer.Contains("skillReset_"))
                        {
                            HandleSkillReset(skill);
                        }
                    }

                    break;
                }
            }

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
            from skill in Skill.List.Except(Skill.Luck.Collect()).Concat(CustomSkill.Loaded.Values)
            where skill.CanReset()
            let costVal = skill.GetResetCost()
            let costStr = costVal > 0
                ? ModEntry.i18n.Get("prestige.dogstatue.cost", new { cost = costVal })
                : string.Empty
            select new Response(skill.StringId, skill.DisplayName + ' ' + costStr)).ToList();

        skillResponses.Add(new Response(
            "cancel",
            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
        location.createQuestionDialogue(
            ModEntry.i18n.Get("prestige.dogstatue.which"),
            skillResponses.ToArray(),
            "skillReset");
    }

    private static void OfferPrestigeRespecChoices(GameLocation location)
    {
        if (ModEntry.Config.Professions.PrestigeRespecCost > 0 && Game1.player.Money < ModEntry.Config.Professions.PrestigeRespecCost)
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
        if (ModEntry.Config.Professions.ChangeUltCost > 0 && Game1.player.Money < ModEntry.Config.Professions.ChangeUltCost)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var chosenUltimate = Game1.player.Get_Ultimate()!;
        var pronoun = chosenUltimate.GetBuffPronoun();
        var message = ModEntry.i18n.Get(
            "prestige.dogstatue.replace",
            new { pronoun, profession = chosenUltimate.Profession.DisplayName, ultimate = chosenUltimate.DisplayName });

        var choices = (
            from unchosenUltimate in Game1.player.GetUnchosenUltimates()
            orderby unchosenUltimate
            let choice = ModEntry.i18n.Get(
                "prestige.dogstatue.choice",
                new { profession = unchosenUltimate.Profession.DisplayName, ultimate = unchosenUltimate.DisplayName })
            select new Response("Choice_" + unchosenUltimate, choice)).ToList();

        choices.Add(new Response("Cancel", ModEntry.i18n.Get("prestige.dogstatue.cancel"))
            .SetHotKey(Keys.Escape));

        location.createQuestionDialogue(message, choices.ToArray(), HandleChangeUlti);
    }

    private static void HandleSkillReset(ISkill skill)
    {
        var cost = skill.GetResetCost();
        if (cost > 0)
        {
            // check for funds and deduct cost
            if (Game1.player.Money < cost)
            {
                Game1.drawObjectDialogue(
                    Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                return;
            }

            Game1.player.Money = Math.Max(0, Game1.player.Money - cost);
        }

        // prepare to prestige at night
        ModEntry.State.Professions.SkillsToReset.Enqueue(skill);
        ModEntry.Events.Enable<PrestigeDayEndingEvent>();

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ModEntry.State.Professions.UsedDogStatueToday = true;
        ModEntry.Events.Enable<PrestigeDayStartedEvent>();
    }

    private static void HandlePrestigeRespect(Skill skill)
    {
        Game1.player.Money = Math.Max(0, Game1.player.Money - (int)ModEntry.Config.Professions.PrestigeRespecCost);

        // remove all prestige professions for this skill
        Enumerable.Range(100 + (skill * 6), 6).ForEach(GameLocation.RemoveProfession);

        var currentLevel = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[0]);
        if (currentLevel >= 15)
        {
            Game1.player.newLevels.Add(new Point(skill, 15));
        }

        if (currentLevel >= 20)
        {
            Game1.player.newLevels.Add(new Point(skill, 20));
        }

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ModEntry.State.Professions.UsedDogStatueToday = true;
        ModEntry.Events.Enable<PrestigeDayStartedEvent>();
    }

    private static void HandleChangeUlti(Farmer who, string choice)
    {
        if (choice == "Cancel")
        {
            return;
        }

        Game1.player.Money = Math.Max(0, Game1.player.Money - (int)ModEntry.Config.Professions.ChangeUltCost);

        // change ultimate
        var chosenUltimate = Ultimate.FromName(choice.Split("_")[1]);
        Game1.player.Set_Ultimate(chosenUltimate);

        // play sound effect
        Sfx.DogStatuePrestige.Play();

        // tell the player
        var pronoun = ModEntry.i18n.Get("article.indefinite" + (Game1.player.IsMale ? ".male" : ".female"));
        Game1.drawObjectDialogue(ModEntry.i18n.Get(
            "prestige.dogstatue.fledged",
            new { pronoun, profession = chosenUltimate.Profession.DisplayName }));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ModEntry.State.Professions.UsedDogStatueToday = true;
        ModEntry.Events.Enable<PrestigeDayStartedEvent>();
    }

    #endregion dialog handlers
}
