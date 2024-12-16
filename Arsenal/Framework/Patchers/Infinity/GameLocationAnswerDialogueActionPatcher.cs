namespace DaLion.Arsenal.Framework.Patchers.Infinity;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

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

    /// <summary>Respond to grab Dark Sword proposition.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(GameLocation __instance, ref bool __result, string? questionAndAnswer)
    {
        if (!CombatModule.Config.Quests.EnableHeroQuest || questionAndAnswer?.StartsWithAnyOf("DarkSword_", "Yoba_") != true)
        {
            return true; // run original logic
        }

        try
        {
            var player = Game1.player;
            switch (questionAndAnswer)
            {
                case "DarkSword_GrabIt":
                    {
                        if (!player.isInventoryFull())
                        {
                            Game1.activeClickableMenu.exitThisMenuNoSound();
                            Game1.playSound("parry");
                            player.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(WeaponIds.DarkSword));
                            player.mailReceived.Add("gotDarkSword");
                        }
                        else
                        {
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                        }

                        break;
                    }

                case "DarkSword_LeaveIt":
                    {
                        break;
                    }

                default:
                    {
                        var split = questionAndAnswer.SplitWithoutAllocation('_');
                        if (!split[0].Equals("Yoba", StringComparison.Ordinal))
                        {
                            return true; // run original logic
                        }

                        switch (split[1].ToString())
                        {
                            case "Honor":
                                Game1.drawObjectDialogue(Virtue.Honor.FlavorText);
                                player.Write(DataKeys.InspectedHonor, true.ToString());
                                break;
                            case "Compassion":
                                Game1.drawObjectDialogue(Virtue.Compassion.FlavorText);
                                player.Write(DataKeys.InspectedCompassion, true.ToString());
                                break;
                            case "Wisdom":
                                Game1.drawObjectDialogue(Virtue.Wisdom.FlavorText);
                                player.Write(DataKeys.InspectedWisdom, true.ToString());
                                break;
                            case "Generosity":
                                Game1.drawObjectDialogue(Virtue.Generosity.FlavorText);
                                player.Write(DataKeys.InspectedGenerosity, true.ToString());
                                break;
                            case "Valor":
                                Game1.drawObjectDialogue(Virtue.Valor.FlavorText);
                                player.Write(DataKeys.InspectedValor, true.ToString());
                                break;
                            case "Yes":
                                if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: WeaponIds.DarkSword } darkSword)
                                {
                                    Log.E("[CMBT]: Player tried to offer a prayer but is not holding the Dark Sword. How did that happen?");
                                    return false; // don't run original logic
                                }

                                var cursePoints = darkSword.Read<int>(DataKeys.CursePoints);
                                Log.D($"Starting with {cursePoints} curse points.");
                                if (cursePoints <= 50)
                                {
                                    Game1.drawObjectDialogue(I18n.Yoba_Prayer_CantHelp());
                                    Log.D("Not enough to get help.");
                                    CombatModule.State.DidPrayToday = true;
                                    return false; // don't run original logic
                                }

                                SoundEffectPlayer.YobaBless.Play(player.currentLocation);
                                Game1.drawObjectDialogue(I18n.Yoba_Prayer_Ok(I18n.Weapons_DarkSword_Name()));
                                cursePoints = (int)((cursePoints - 50) * 0.8) + 50;
                                Log.D($"Ending with {cursePoints} curse points.");
                                darkSword.Write(DataKeys.CursePoints, cursePoints.ToString());
                                CombatModule.State.DidPrayToday = true;
                                return false; // don't run original logic
                            case "No":
                                return false; // don't run original logic
                        }

                        if (!player.Read<bool>(DataKeys.InspectedHonor) ||
                            !player.Read<bool>(DataKeys.InspectedCompassion) ||
                            !player.Read<bool>(DataKeys.InspectedWisdom) ||
                            !player.Read<bool>(DataKeys.InspectedGenerosity) ||
                            !player.Read<bool>(DataKeys.InspectedValor))
                        {
                            Game1.afterDialogues = () =>
                            {
                                var question = I18n.Yoba_Inscriptions();
                                var responses = Virtue.List
                                    .Select(v => new Response(v.Name, v.DisplayName))
                                    .ToArray();
                                __instance.createQuestionDialogue(question, responses, "Yoba");
                            };

                            return false; // don't run original logic
                        }

                        player.completeQuest((int)QuestId.CurseIntro);
                        CombatModule.State.HeroQuest ??= new HeroQuest();

                        if (!Context.IsMainPlayer)
                        {
                            if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade") &&
                                player.Read<int>(Virtue.Generosity.Name) < 5e5)
                            {
                                player.Increment(Virtue.Generosity.Name, 5e5);
                                CombatModule.State.HeroQuest.UpdateTrialProgress(Virtue.Generosity);
                            }
                            else if (Game1.MasterPlayer.mailReceived.Contains("communityUpgradeShortcuts") &&
                                     player.Read<int>(Virtue.Generosity.Name) < 3e5)
                            {
                                player.Increment(Virtue.Generosity.Name, 3e3);
                                CombatModule.State.HeroQuest.UpdateTrialProgress(Virtue.Generosity);
                            }
                        }

                        player.Write(DataKeys.InspectedHonor, null);
                        player.Write(DataKeys.InspectedCompassion, null);
                        player.Write(DataKeys.InspectedWisdom, null);
                        player.Write(DataKeys.InspectedGenerosity, null);
                        player.Write(DataKeys.InspectedValor, null);
                        player.Write(DataKeys.VirtueQuestState, HeroQuest.QuestState.InProgress.ToString());
                        return false; // don't run original logic
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
}
