namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Collections;
using Events.GameLoop;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ultimates;
using VirtualProperties;
using Localization = Utility.Localization;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialogueActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationAnswerDialogueActionPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, string questionAndAnswer)
    {
        if (!ModEntry.Config.EnablePrestige ||
            (!questionAndAnswer.Contains("dogStatue") || questionAndAnswer.Contains("No")) &&
            !questionAndAnswer.ContainsAnyOf("prestigeRespec_", "skillReset_"))
            return true; // run original logic

        try
        {
            switch (questionAndAnswer)
            {
                case "dogStatue_Yes":
                    {
                        var skillResponses = (
                            from skill in Skill.List.Except(Skill.Luck.Collect()).Concat(ModEntry.CustomSkills.Values)
                            where Game1.player.CanResetSkill(skill)
                            let costVal = Game1.player.GetResetCost(skill)
                            let costStr = costVal > 0
                                ? ModEntry.i18n.Get("prestige.dogstatue.cost", new { cost = costVal })
                                : string.Empty
                            select new Response(skill.StringId, skill.DisplayName + ' ' + costStr)).ToList();

                        skillResponses.Add(new("cancel",
                            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
                        __instance.createQuestionDialogue(ModEntry.i18n.Get("prestige.dogstatue.which"),
                            skillResponses.ToArray(), "skillReset");
                        break;
                    }
                case "dogStatue_prestigeRespec" when ModEntry.Config.PrestigeRespecCost > 0 &&
                                                     Game1.player.Money < ModEntry.Config.PrestigeRespecCost:
                    {
                        Game1.drawObjectDialogue(
                            Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                        return false; // don't run original logic
                    }
                case "dogStatue_prestigeRespec":
                    {
                        var skillResponses = new List<Response>();
                        if (GameLocation.canRespec(Skill.Farming))
                            skillResponses.Add(new("farming",
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));

                        if (GameLocation.canRespec(Skill.Fishing))
                            skillResponses.Add(new("fishing",
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));

                        if (GameLocation.canRespec(Skill.Foraging))
                            skillResponses.Add(new("foraging",
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));

                        if (GameLocation.canRespec(Skill.Mining))
                            skillResponses.Add(new("mining",
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));

                        if (GameLocation.canRespec(Skill.Combat))
                            skillResponses.Add(new("combat",
                                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608")));

                        skillResponses.Add(new("cancel",
                            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
                        __instance.createQuestionDialogue(
                            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueQuestion"),
                            skillResponses.ToArray(), "prestigeRespec");
                        break;
                    }
                case "dogStatue_changeUlt" when ModEntry.Config.ChangeUltCost > 0 &&
                                                Game1.player.Money < ModEntry.Config.ChangeUltCost:
                    {
                        Game1.drawObjectDialogue(
                            Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                        return false; // don't run original logic
                    }
                case "dogStatue_changeUlt":
                    {
                        var currentProfessionKey =
                            Game1.player.get_Ultimate()!.Index.ToString().SplitCamelCase()[0].ToLowerInvariant();
                        var currentProfessionDisplayName =
                            ModEntry.i18n.Get(currentProfessionKey + ".name.male");
                        var currentUlti = ModEntry.i18n.Get(currentProfessionKey + ".ulti.name");
                        var pronoun = Localization.GetBuffPronoun();
                        var message = ModEntry.i18n.Get("prestige.dogstatue.replace",
                            new { pronoun, currentProfession = currentProfessionDisplayName, currentUlti });

                        var choices = (
                            from superModeIndex in Game1.player.GetUnchosenUltimates()
                            orderby superModeIndex
                            let choiceProfessionKey = superModeIndex.ToString().SplitCamelCase()[0].ToLowerInvariant()
                            let choiceProfessionDisplayName =
                                ModEntry.i18n.Get(choiceProfessionKey + ".name.male")
                            let choiceUlti = ModEntry.i18n.Get(choiceProfessionKey + ".ulti.name" +
                                                               (superModeIndex == UltimateIndex.PiperConcerto
                                                                   ? Game1.player.IsMale ? ".male" : ".female"
                                                                   : string.Empty))
                            let choice =
                                ModEntry.i18n.Get("prestige.dogstatue.choice",
                                    new { choiceProfession = choiceProfessionDisplayName, choiceBuff = choiceUlti })
                            select new Response("Choice_" + superModeIndex, choice)).ToList();

                        choices.Add(new Response("Cancel", ModEntry.i18n.Get("prestige.dogstatue.cancel"))
                            .SetHotKey(Keys.Escape));

                        __instance.createQuestionDialogue(message, choices.ToArray(), delegate (Farmer _, string choice)
                        {
                            if (choice == "Cancel") return;

                            Game1.player.Money = Math.Max(0, Game1.player.Money - (int)ModEntry.Config.ChangeUltCost);

                            // change ultimate
                            var newIndex = Enum.Parse<UltimateIndex>(choice.Split("_")[1]);
                            Game1.player.set_Ultimate(Ultimate.FromIndex(newIndex));

                            // play sound effect
                            SFX.DogStatuePrestige.Play();

                            // tell the player
                            var choiceProfessionKey = newIndex.ToString().ToLowerInvariant();
                            var choiceProfessionDisplayName =
                                ModEntry.i18n.Get(choiceProfessionKey +
                                                  (Game1.player.IsMale ? ".name.male" : ".name.female"));
                            pronoun = ModEntry.i18n.Get("pronoun.indefinite" +
                                                        (Game1.player.IsMale ? ".male" : ".female"));
                            Game1.drawObjectDialogue(ModEntry.i18n.Get("prestige.dogstatue.fledged",
                                new { pronoun, choiceProfession = choiceProfessionDisplayName }));

                            // woof woof
                            DelayedAction.playSoundAfterDelay("dog_bark", 1300);
                            DelayedAction.playSoundAfterDelay("dog_bark", 1900);

                            ModEntry.State.UsedDogStatueToday = true;
                            ModEntry.Events.Enable<PrestigeDayStartedEvent>();
                        });
                        return false; // don't run original logic
                    }
                default:
                    {
                        // if cancel do nothing
                        var skillName = questionAndAnswer.Split('_')[1];
                        if (skillName is "cancel" or "Yes") return false; // don't run original logic

                        // get skill type and do action
                        if (Skill.TryFromName(skillName, true, out var skill))
                        {
                            if (questionAndAnswer.Contains("skillReset_"))
                            {
                                var cost = Game1.player.GetResetCost(skill);
                                if (cost > 0)
                                {
                                    // check for funds and deduct cost
                                    if (Game1.player.Money < cost)
                                    {
                                        Game1.drawObjectDialogue(
                                            Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                                        return false; // don't run original logic
                                    }

                                    Game1.player.Money = Math.Max(0, Game1.player.Money - cost);
                                }

                                // prepare to prestige at night
                                ModEntry.State.SkillsToReset.Enqueue(skill);
                                ModEntry.Events.Enable<PrestigeDayEndingEvent>();

                                // play sound effect
                                SFX.DogStatuePrestige.Play();

                                // tell the player
                                Game1.drawObjectDialogue(
                                    Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                            }
                            else if (questionAndAnswer.Contains("prestigeRespec_"))
                            {
                                Game1.player.Money = Math.Max(0,
                                    Game1.player.Money - (int)ModEntry.Config.PrestigeRespecCost);

                                // remove all prestige professions for this skill
                                Enumerable.Range(100 + skill * 6, 6).ForEach(GameLocation.RemoveProfession);

                                var currentLevel = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[0]);
                                if (currentLevel >= 15)
                                    Game1.player.newLevels.Add(new(skill, 15));
                                if (currentLevel >= 20)
                                    Game1.player.newLevels.Add(new(skill, 20));

                                // play sound effect
                                SFX.DogStatuePrestige.Play();

                                // tell the player
                                Game1.drawObjectDialogue(
                                    Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

                                ModEntry.State.UsedDogStatueToday = true;
                                ModEntry.Events.Enable<PrestigeDayStartedEvent>();
                            }
                        }
                        else if (ModEntry.CustomSkills.TryGetValue(skillName, out var customSkill))
                        {
                            if (questionAndAnswer.Contains("skillReset_"))
                            {
                                var cost = Game1.player.GetResetCost(customSkill);
                                if (cost > 0)
                                {
                                    // check for funds and deduct cost
                                    if (Game1.player.Money < cost)
                                    {
                                        Game1.drawObjectDialogue(
                                            Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
                                        return false; // don't run original logic
                                    }

                                    Game1.player.Money = Math.Max(0, Game1.player.Money - cost);
                                }

                                // prepare to prestige at night
                                ModEntry.State.SkillsToReset.Enqueue(customSkill);
                                ModEntry.Events.Enable<PrestigeDayEndingEvent>();

                                // play sound effect
                                SFX.DogStatuePrestige.Play();

                                // tell the player
                                Game1.drawObjectDialogue(
                                    Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));
                            }
                        }

                        // woof woof
                        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
                        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

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
}