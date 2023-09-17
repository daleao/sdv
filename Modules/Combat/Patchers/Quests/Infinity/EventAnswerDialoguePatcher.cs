namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Infinity;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class EventAnswerDialoguePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventAnswerDialoguePatcher"/> class.</summary>
    internal EventAnswerDialoguePatcher()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.answerDialogue));
    }

    #region harmony patches

    /// <summary>Record virtues after dialogue response. This works for questions triggered by the `question` event command.</summary>
    [HarmonyPostfix]
    private static void EventAnswerDialoguePostfix(Event __instance, int answerChoice)
    {
        var player = Game1.player;
        var amount = 0;
        Virtue? virtue = null;
        switch (__instance.id)
        {
        // HONOR //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 0:

            // Sam 3 hearts | Location: Beach
            // Alex 4 hearts | Location: Town
            case 733330 or 2481135 when answerChoice == 0:

            // Sophia 2 hearts | Location: Custom_BlueMoonVineyard
            case 8185291 when answerChoice == 1:

                virtue = Virtue.Honor;
                amount = 1;
                break;

        // COMPASSION //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 1:

            // Sam 3 hearts | Location: Beach
            case 733330 when answerChoice == 1:

            // Pam 9 hearts | Location: Trailer_Big
            case 503180 when answerChoice == 0:

            // Shane 6 hearts | Location: Forest
            case 3910975 when answerChoice is 1 or 3:

            // Sophia 6 hearts | Location: Custom_BlueMoonVineyard
            case 8185294 when answerChoice == 0:

            // Sebastian Mature Event | Location: Mountain
            case 1000005 when answerChoice == 0 && __instance.CurrentCommand == 37:

            // Caroline Mature Event | Location: Forest
            case 1000013 when answerChoice == 0:

                virtue = Virtue.Compassion;
                amount = 1;
                break;

            // Pam 9 hearts | Location: Trailer_Big
            case 503180 when answerChoice == 1:

            // Sebastian Mature Event | Location: Mountain
            case 1000005 when answerChoice == 0 && __instance.CurrentCommand == 3:

                virtue = Virtue.Compassion;
                amount = -1;
                break;

        // WISDOM //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 2:

            // Jas Mature Event | Location: Forest
            case 1000021 when answerChoice == 0:

                virtue = Virtue.Wisdom;
                amount = 1;
                break;

        // GENEROSITY //

            // Claire 2 hearts | Location: Saloon
            case 3219871:

                virtue = Virtue.Generosity;
                amount = 2200;
                break;
        }

        if (virtue is null)
        {
            return;
        }

        switch (amount)
        {
            case > 0:
                Game1.chatBox.addMessage(
                    __instance.actors.Count > 1
                        ? I18n.Virtues_Appreciate_Plural(
                            __instance.actors[0].displayName,
                            __instance.actors[1].displayName,
                            Virtue.Honor.DisplayName)
                        : I18n.Virtues_Appreciate_Singular(__instance.actors[0].displayName, virtue.DisplayName),
                    Color.Green);
                player.Increment(virtue.Name, amount);
                CombatModule.State.HeroQuest?.UpdateTrialProgress(virtue);
                break;
            case < 0:
            {
                Game1.chatBox.addMessage(
                    __instance.actors.Count > 1
                        ? I18n.Virtues_Disapprove_Plural(
                            __instance.actors[0].displayName,
                            __instance.actors[1].displayName,
                            Virtue.Honor.DisplayName)
                        : I18n.Virtues_Disapprove_Singular(__instance.actors[0].displayName, virtue.DisplayName),
                    Color.Green);
                player.Increment(virtue.Name, amount);
                if (player.Read<int>(virtue.Name) < 0)
                {
                    player.Write(virtue.Name, 0.ToString());
                }

                CombatModule.State.HeroQuest?.UpdateTrialProgress(virtue);
                break;
            }
        }
    }

    #endregion harmony patches
}
