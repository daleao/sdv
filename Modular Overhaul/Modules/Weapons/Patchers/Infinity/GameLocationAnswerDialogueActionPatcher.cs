namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Reflection;
using DaLion.Overhaul;
using DaLion.Overhaul.Modules.Weapons;
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

    /// <summary>Respond to grab Dark Sword proposition + blacksmith forge.</summary>
    [HarmonyPrefix]
    private static bool GameLocationAnswerDialogueActionPrefix(ref bool __result, string? questionAndAnswer)
    {
        if (questionAndAnswer is null)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (!WeaponsModule.Config.InfinityPlusOne && !WeaponsModule.Config.DwarvenLegacy)
        {
            return true; // run original logic
        }

        if (!questionAndAnswer.ContainsAnyOf("DarkSword_", "Yoba_"))
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
                        Game1.activeClickableMenu.exitThisMenuNoSound();
                        Game1.playSound("parry");
                        player.addItemByMenuIfNecessaryElseHoldUp(new MeleeWeapon(ItemIDs.DarkSword));
                        player.mailReceived.Add("gotDarkSword");
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
                        }

                        if (!player.Read<bool>(DataKeys.InspectedHonor) ||
                            !player.Read<bool>(DataKeys.InspectedCompassion) ||
                            !player.Read<bool>(DataKeys.InspectedWisdom) ||
                            !player.Read<bool>(DataKeys.InspectedGenerosity) ||
                            !player.Read<bool>(DataKeys.InspectedValor))
                        {
                            return false; // don't run original logic
                        }

                        player.completeQuest((int)Quest.CurseIntro);
                        WeaponsModule.State.VirtuesQuest ??= new VirtuesQuest();

                        player.Write(DataKeys.InspectedHonor, null);
                        player.Write(DataKeys.InspectedCompassion, null);
                        player.Write(DataKeys.InspectedWisdom, null);
                        player.Write(DataKeys.InspectedGenerosity, null);
                        player.Write(DataKeys.InspectedValor, null);
                        player.Write(DataKeys.VirtueQuestState, VirtuesQuestState.InProgress.ToString());
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
