namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatcher"/> class.</summary>
    internal GameLocationPerformActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ProfessionsModule.Config.EnablePrestige || action?.Contains("DogStatue") != true ||
            !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (!ProfessionsModule.Config.AllowMultipleResets && ProfessionsModule.State.SkillsToReset.Count > 0)
            {
                message = I18n.Prestige_DogStatue_Dismiss();
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (ISkill.CanResetAny())
            {
                OfferSkillReset(__instance);
                return false; // don't run original logic
            }

            if (who.HasAllProfessions(true))
            {
                OfferRespecOptions(__instance);
                return false; // don't run original logic
            }

            message = I18n.Prestige_DogStatue_First();
            Game1.drawObjectDialogue(message);
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

    private static void OfferSkillReset(GameLocation location)
    {
        var message = I18n.Prestige_DogStatue_First();
        if (ProfessionsModule.Config.ForgetRecipesOnSkillReset)
        {
            message += I18n.Prestige_DogStatue_Forget();
        }

        message += I18n.Prestige_DogStatue_Offer();

        location.createQuestionDialogue(message, location.createYesNoResponses(), "dogStatue");
    }

    private static void OfferRespecOptions(GameLocation location)
    {
        var message = I18n.Prestige_DogStatue_What();
        var options = Array.Empty<Response>();

        if (ProfessionsModule.Config.EnableLimitBreaks &&
            Game1.player.Get_Ultimate() is not null)
        {
            options = options.Concat(new Response[]
            {
                new(
                    "changeUlt",
                    I18n.Prestige_DogStatue_Changeult() +
                    (ProfessionsModule.Config.LimitRespecCost > 0
                        ? ' ' + I18n.Prestige_DogStatue_Cost(ProfessionsModule.Config.LimitRespecCost)
                        : string.Empty)),
            }).ToArray();
        }

        if (Skill.List.Any(s => GameLocation.canRespec(s)))
        {
            options = options.Concat(new Response[]
            {
                new(
                    "prestigeRespec",
                    I18n.Prestige_DogStatue_Respec() +
                    (ProfessionsModule.Config.PrestigeRespecCost > 0
                        ? ' ' + I18n.Prestige_DogStatue_Cost(ProfessionsModule.Config.PrestigeRespecCost)
                        : string.Empty)),
            }).ToArray();
        }

        location.createQuestionDialogue(message, options, "dogStatue");
    }

    #endregion dialog handlers
}
