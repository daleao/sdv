namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
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
    internal GameLocationAnswerDialogueActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogueAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, ref bool __result, string? questionAndAnswer)
    {
        if ((!ProfessionsModule.EnableSkillReset ||
             questionAndAnswer?.StartsWithAnyOf("dogStatue", "prestigeRespec", "skillReset") != true) &&
            (ProfessionsModule.Config.PrestigeProgressionMode != ProfessionConfig.PrestigeMode.Streamlined ||
             questionAndAnswer?.StartsWith("professionForget") != true))
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
                {
                    OfferChangeUltiChoices(__instance);
                    break;
                }

                default:
                {
                    // if cancel do nothing
                    var skillNameAsSpan = questionAndAnswer.SplitWithoutAllocation('_')[1];
                    if (skillNameAsSpan.Equals("cancel", StringComparison.Ordinal) || skillNameAsSpan.Equals("Yes", StringComparison.Ordinal))
                    {
                        __result = true;
                        return false; // don't run original logic
                    }

                    // get skill type and do action
                    var skillName = skillNameAsSpan.ToString();
                    if (VanillaSkill.TryFromName(skillName, true, out var skill))
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
                            HandlePrestigeRespec((CustomSkill)customSkill);
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
            from skill in VanillaSkill.List.Except(VanillaSkill.Luck.Collect()).Concat(CustomSkill.Loaded.Values)
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
        if (ProfessionsModule.Config.PrestigeRespecCost > 0 && Game1.player.Money < ProfessionsModule.Config.PrestigeRespecCost)
        {
            Game1.drawObjectDialogue(
                Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var skillResponses = new List<Response>();
        if (GameLocation.canRespec(VanillaSkill.Farming))
        {
            skillResponses.Add(new Response(
                "farming",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604")));
        }

        if (GameLocation.canRespec(VanillaSkill.Fishing))
        {
            skillResponses.Add(new Response(
                "fishing",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607")));
        }

        if (GameLocation.canRespec(VanillaSkill.Foraging))
        {
            skillResponses.Add(new Response(
                "foraging",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606")));
        }

        if (GameLocation.canRespec(VanillaSkill.Mining))
        {
            skillResponses.Add(new Response(
                "mining",
                Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605")));
        }

        if (GameLocation.canRespec(VanillaSkill.Combat))
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

    private static void OfferChangeUltiChoices(GameLocation location)
    {
        if (ProfessionsModule.Config.LimitRespecCost > 0 && Game1.player.Money < ProfessionsModule.Config.LimitRespecCost)
        {
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            return;
        }

        var chosenUltimate = Game1.player.Get_Ultimate()!;
        var choices = (
            from unchosenUltimate in Game1.player.GetUnchosenUltimates()
            orderby unchosenUltimate
            let choice = I18n.Prestige_DogStatue_Choice(unchosenUltimate.Profession.Title, unchosenUltimate.DisplayName)
            select new Response("Choice_" + unchosenUltimate, choice)).ToList();
        choices.Add(new Response("Cancel", I18n.Prestige_DogStatue_Cancel())
            .SetHotKey(Keys.Escape));

        var message = I18n.Prestige_DogStatue_Replace(chosenUltimate.Profession.Title, chosenUltimate.DisplayName);
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
        ProfessionsModule.State.SkillsToReset.Enqueue(skill);

        // play sound effect
        SoundEffectPlayer.DogStatuePrestige.Play(player.currentLocation);

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ProfessionsModule.State.UsedStatueToday = true;
    }

    private static void HandlePrestigeRespec(VanillaSkill skill)
    {
        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)ProfessionsModule.Config.PrestigeRespecCost);

        // remove all prestige professions for this skill
        for (var i = 0; i < 6; i++)
        {
            GameLocation.RemoveProfession(100 + (skill * 6) + i);
        }

        var currentLevel = skill.CurrentLevel;
        if (ProfessionsModule.Config.PrestigeProgressionMode == ProfessionConfig.PrestigeMode.Streamlined)
        {
            // also remove regular professions
            for (var i = 0; i < 6; i++)
            {
                GameLocation.RemoveProfession((skill * 6) + i);
            }

            // re-add levels
            if (currentLevel >= 5)
            {
                player.newLevels.Add(new Point(skill, 5));
            }

            if (currentLevel >= 10)
            {
                player.newLevels.Add(new Point(skill, 10));
            }
        }

        // re-add prestige levels
        if (currentLevel >= 15)
        {
            player.newLevels.Add(new Point(skill, 15));
        }

        if (currentLevel >= 20)
        {
            player.newLevels.Add(new Point(skill, 20));
        }

        // play sound effect
        SoundEffectPlayer.DogStatuePrestige.Play(player.currentLocation);

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ProfessionsModule.State.UsedStatueToday = true;
    }

    private static void HandlePrestigeRespec(CustomSkill skill)
    {
        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)ProfessionsModule.Config.PrestigeRespecCost);

        // remove all prestige professions for this skill
        for (var i = 0; i < 6; i++)
        {
            GameLocation.RemoveProfession(100 + skill.Professions[i].Id);
        }

        var currentLevel = Farmer.checkForLevelGain(0, player.experiencePoints[0]);
        if (ProfessionsModule.Config.PrestigeProgressionMode == ProfessionConfig.PrestigeMode.Streamlined)
        {
            // also remove regular professions
            for (var i = 0; i < 6; i++)
            {
                GameLocation.RemoveProfession(100 + skill.Professions[i].Id);
            }

            // re-add levels
            if (currentLevel >= 5)
            {
                Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                    .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 5));
            }

            if (currentLevel >= 10)
            {
                Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                    .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 10));
            }
        }

        // re-add presige levels
        if (currentLevel >= 15)
        {
            Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 15));
        }

        if (currentLevel >= 20)
        {
            Reflector.GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SpaceCore.Skills), "NewLevels")
                .Invoke().Add(new KeyValuePair<string, int>(skill.StringId, 20));
        }

        // play sound effect
        SoundEffectPlayer.DogStatuePrestige.Play(player.currentLocation);

        // tell the player
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Sewer_DogStatueFinished"));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ProfessionsModule.State.UsedStatueToday = true;
    }

    private static void HandleChangeUlti(Farmer who, string choice)
    {
        if (choice == "Cancel")
        {
            return;
        }

        var player = Game1.player;
        player.Money = Math.Max(0, player.Money - (int)ProfessionsModule.Config.LimitRespecCost);

        // change ultimate
        var chosenUltimate = Ultimate.FromName(choice.SplitWithoutAllocation('_')[1].ToString());
        player.Set_Ultimate(chosenUltimate);

        // play sound effect
        SoundEffectPlayer.DogStatuePrestige.Play(player.currentLocation);

        // tell the player
        Game1.drawObjectDialogue(I18n.Prestige_DogStatue_Fledged(chosenUltimate.Profession.Title));

        // woof woof
        DelayedAction.playSoundAfterDelay("dog_bark", 1300);
        DelayedAction.playSoundAfterDelay("dog_bark", 1900);

        ProfessionsModule.State.UsedStatueToday = true;
    }

    #endregion dialog handlers
}
