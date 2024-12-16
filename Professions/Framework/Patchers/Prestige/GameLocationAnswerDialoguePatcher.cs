namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Reflection;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationAnswerDialoguePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationAnswerDialoguePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationAnswerDialoguePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.answerDialogue));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Transcendance.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.VeryHigh)]
    private static bool GameLocationAnswerDialoguePrefix(
        GameLocation __instance,
        ref bool __result,
        Response answer)
    {
        if (__instance.lastQuestionKey != "ConfirmChangeLimit")
        {
            return true; // run original logic
        }

        var choice = answer.responseKey;
        if (choice == "Return")
        {
            __instance.lastQuestionKey = "dogStatue_changeUlt";
            return true; // don't run original logic
        }

        var split = choice.Split('_');
        if (split.Length != 2)
        {
            __result = false;
            return false; // don't run original logic
        }

        try
        {
            var player = Game1.player;
            player.Money = Math.Max(0, player.Money - (int)Config.Masteries.LimitRespecCost);

            // change Limit Break
            var newLimit = LimitBreak.FromName(split[1]);
            player.Set_LimitBreak(newLimit);

            // play sound effect
            SoundBox.DogStatuePrestige.PlayLocal();

            // tell the player
            Game1.drawObjectDialogue(I18n.Prestige_DogStatue_Fledged(newLimit.ParentProfession.GetTitle(false)));

            // woof woof
            DelayedAction.playSoundAfterDelay("dog_bark", 1300);
            DelayedAction.playSoundAfterDelay("dog_bark", 1900);

            State.UsedStatueToday = true;
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
