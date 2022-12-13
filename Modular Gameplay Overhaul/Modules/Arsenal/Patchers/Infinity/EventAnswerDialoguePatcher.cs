namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

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
        switch (__instance.id)
        {
        // HONOR //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 0:

            // Sam 3 hearts | Location: Beach
            // Alex 4 hearts | Location: Town
            case 733330 or 2481135 when answerChoice == 0:
                Game1.player.Increment(DataFields.ProvenHonor);
                return;

        // COMPASSION //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 1:

            // Sam 3 hearts | Location: Beach
            case 733330 when answerChoice == 1:

            // Pam 9 hearts | Location: Trailer_Big
            case 503180 when answerChoice == 0:

            // Shane 6 hearts | Location: Forest
            case 3910975 when answerChoice is 1 or 3:
                Game1.player.Increment(DataFields.ProvenCompassion);
                return;

            // Pam 9 hearts | Location: Trailer_Big
            case 503180 when answerChoice == 1:

                Game1.player.Increment(DataFields.ProvenCompassion, -1);
                return;

            // WISDOM //

            // Sebastian 6 hearts | Location: SebastianRoom
            case 27 when answerChoice == 2:

                Game1.player.Increment(DataFields.ProvenWisdom);
                return;
        }
    }

    #endregion harmony patches
}
