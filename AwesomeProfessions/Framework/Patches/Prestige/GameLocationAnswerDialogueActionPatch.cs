namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Enums;
using StardewValley;

using Stardew.Common.Extensions;
using AssetLoaders;
using Events.GameLoop;
using Extensions;
using SuperMode;

using Localization = Utility.Localization;

#endregion using directives

[UsedImplicitly]
internal class GameLocationAnswerDialogueActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationAnswerDialogueActionPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
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
                    var skillResponses = new List<Response>();
                    if (Game1.player.CanResetSkill(SkillType.Farming))
                    {
                        var costVal = Game1.player.GetResetCost(SkillType.Farming);
                        var costStr = costVal > 0
                            ? ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal})
                            : string.Empty;
                        skillResponses.Add(new("farming",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604") + ' ' +
                            costStr));
                    }

                    if (Game1.player.CanResetSkill(SkillType.Fishing))
                    {
                        var costVal = Game1.player.GetResetCost(SkillType.Fishing);
                        var costStr = costVal > 0
                            ? ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal})
                            : string.Empty;
                        skillResponses.Add(new("fishing",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607") + ' ' +
                            costStr));
                    }

                    if (Game1.player.CanResetSkill(SkillType.Foraging))
                    {
                        var costVal = Game1.player.GetResetCost(SkillType.Foraging);
                        var costStr = costVal > 0
                            ? ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal})
                            : string.Empty;
                        skillResponses.Add(new("foraging",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606") + ' ' +
                            costStr));
                    }

                    if (Game1.player.CanResetSkill(SkillType.Mining))
                    {
                        var costVal = Game1.player.GetResetCost(SkillType.Mining);
                        var costStr = costVal > 0
                            ? ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal})
                            : string.Empty;
                        skillResponses.Add(new("mining",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605") + ' ' +
                            costStr));
                    }

                    if (Game1.player.CanResetSkill(SkillType.Combat))
                    {
                        var costVal = Game1.player.GetResetCost(SkillType.Combat);
                        var costStr = costVal > 0
                            ? ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost", new {cost = costVal})
                            : string.Empty;
                        skillResponses.Add(new("combat",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608") + ' ' +
                            costStr));
                    }

                    skillResponses.Add(new("cancel",
                        Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueCancel")));
                    __instance.createQuestionDialogue(ModEntry.ModHelper.Translation.Get("prestige.dogstatue.which"),
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
                    if (GameLocation.canRespec((int) SkillType.Farming))
                        skillResponses.Add(new("farming",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));

                    if (GameLocation.canRespec((int) SkillType.Fishing))
                        skillResponses.Add(new("fishing",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));

                    if (GameLocation.canRespec((int) SkillType.Foraging))
                        skillResponses.Add(new("foraging",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));

                    if (GameLocation.canRespec((int) SkillType.Mining))
                        skillResponses.Add(new("mining",
                            Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));

                    if (GameLocation.canRespec((int) SkillType.Combat))
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
                    var currentProfessionKey = ModEntry.PlayerState.Value.SuperMode.Index.ToString().ToLower();
                    var currentProfessionDisplayName =
                        ModEntry.ModHelper.Translation.Get(currentProfessionKey + ".name.male");
                    var currentBuff = ModEntry.ModHelper.Translation.Get(currentProfessionKey + ".buff");
                    var pronoun = Localization.GetBuffPronoun();
                    var message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.replace",
                        new {pronoun, currentProfession = currentProfessionDisplayName, currentBuff});

                    var choices = (
                        from superModeIndex in Game1.player.GetUnchosenSuperModes()
                        orderby superModeIndex
                        let choiceProfessionKey = superModeIndex.ToString().ToLower()
                        let choiceProfessionDisplayName =
                            ModEntry.ModHelper.Translation.Get(choiceProfessionKey + ".name.male")
                        let choiceBuff = ModEntry.ModHelper.Translation.Get(choiceProfessionKey + ".buff")
                        let choice =
                            ModEntry.ModHelper.Translation.Get("prestige.dogstatue.choice",
                                new {choiceProfession = choiceProfessionDisplayName, choiceBuff})
                        select new Response("Choice_" + superModeIndex, choice)).ToList();

                    choices.Add(new Response("Cancel", Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No"))
                        .SetHotKey(Keys.Escape));

                    __instance.createQuestionDialogue(message, choices.ToArray(), delegate(Farmer _, string choice)
                    {
                        if (choice == "Cancel") return;

                        Game1.player.Money = Math.Max(0, Game1.player.Money - (int)ModEntry.Config.ChangeUltCost);

                        // change super mode
                        var newIndex = Enum.Parse<SuperModeIndex>(choice.Split("_")[1]);
                        ModEntry.PlayerState.Value.SuperMode =
#pragma warning disable CS8509
                            ModEntry.PlayerState.Value.SuperMode = newIndex switch
#pragma warning restore CS8509
                            {
                                SuperModeIndex.Brute => new BruteFury(),
                                SuperModeIndex.Poacher => new PoacherColdBlood(),
                                SuperModeIndex.Piper => new PiperEubstance(),
                                SuperModeIndex.Desperado => new DesperadoTemerity()
                            };
                        Game1.player.WriteData(DataField.SuperModeIndex, newIndex.ToString());

                        // play sound effect
                        SoundBank.Play((SFX)SFX.DogStatuePrestige);

                        // tell the player
                        var choiceProfessionKey = newIndex.ToString().ToLower();
                        var choiceProfessionDisplayName =
                            ModEntry.ModHelper.Translation.Get(choiceProfessionKey +
                                                               (Game1.player.IsMale ? ".name.male" : ".name.female"));
                        pronoun = ModEntry.ModHelper.Translation.Get("pronoun.indefinite" +
                                                                     (Game1.player.IsMale ? ".male" : ".female"));
                        Game1.drawObjectDialogue(ModEntry.ModHelper.Translation.Get("prestige.dogstatue.fledged",
                            new { pronoun, choiceProfession = choiceProfessionDisplayName}));

                        // woof woof
                        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
                        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

                        ModEntry.PlayerState.Value.UsedDogStatueToday = true;
                    });
                    return false; // don't run original logic
                }
                default:
                {
                    // if cancel do nothing
                    var skillName = questionAndAnswer.Split('_')[1];
                    if (skillName == "cancel") return false; // don't run original logic

                    // get skill type
#pragma warning disable CS8509
                    var skillType = skillName switch
#pragma warning restore CS8509
                    {
                        "farming" => SkillType.Farming,
                        "fishing" => SkillType.Fishing,
                        "foraging" => SkillType.Foraging,
                        "mining" => SkillType.Mining,
                        "combat" => SkillType.Combat
                    };

                    if (questionAndAnswer.Contains("skillReset_"))
                    {
                        var cost = Game1.player.GetResetCost(skillType);
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
                        var prestigeDayEndingEvent = EventManager.Get<PrestigeDayEndingEvent>();
                        prestigeDayEndingEvent.SkillsToReset.Value.Enqueue(skillType);
                        prestigeDayEndingEvent.Enable();

                        // play sound effect
                        SoundBank.Play(SFX.DogStatuePrestige);

                        // tell the player
                        Game1.drawObjectDialogue(
                            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

                        // woof woof
                        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
                        DelayedAction.playSoundAfterDelay("dog_bark", 1900);
                    }
                    else if (questionAndAnswer.Contains("prestigeRespec_"))
                    {
                        Game1.player.Money = Math.Max(0, Game1.player.Money - (int) ModEntry.Config.PrestigeRespecCost);

                        // remove all prestige professions for this skill
                        Enumerable.Range(100 + (int) skillType * 6, 6).ForEach(GameLocation.RemoveProfession);

                        var currentLevel = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[0]);
                        if (currentLevel >= 15)
                            Game1.player.newLevels.Add(new((int) skillType, 15));
                        if (currentLevel >= 20)
                            Game1.player.newLevels.Add(new((int) skillType, 20));

                        // play sound effect
                        SoundBank.Play(SFX.DogStatuePrestige);

                        // tell the player
                        Game1.drawObjectDialogue(
                            Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

                        // woof woof
                        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
                        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

                        ModEntry.PlayerState.Value.UsedDogStatueToday = true;
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
}