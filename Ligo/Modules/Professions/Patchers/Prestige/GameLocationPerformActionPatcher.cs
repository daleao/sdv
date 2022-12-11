namespace DaLion.Ligo.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
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
        if (!ProfessionsModule.Config.EnablePrestige || action?.StartsWith("DogStatue") != true ||
            !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (!ProfessionsModule.Config.AllowMultiplePrestige && who.Get_HasSkillsToReset())
            {
                message = i18n.Get("prestige.dogstatue.dismiss");
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (ISkill.CanResetAny())
            {
                OfferSkillReset(__instance);
                return false; // don't run original logic
            }

            if (who.HasAllProfessions())
            {
                OfferRespecOptions(__instance);
                return false; // don't run original logic
            }

            message = i18n.Get("prestige.dogstatue.first");
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
        string message = i18n.Get("prestige.dogstatue.first");
        if (ProfessionsModule.Config.ForgetRecipes)
        {
            message += i18n.Get("prestige.dogstatue.forget");
        }

        message += i18n.Get("prestige.dogstatue.offer");

        location.createQuestionDialogue(message, location.createYesNoResponses(), "dogStatue");
    }

    private static void OfferRespecOptions(GameLocation location)
    {
        string message = i18n.Get("prestige.dogstatue.what");
        var options = Array.Empty<Response>();

        if (Game1.player.Get_Ultimate() is not null)
        {
            options = options.Concat(new Response[]
            {
                new(
                    "changeUlt",
                    i18n.Get("prestige.dogstatue.changeult") +
                    (ProfessionsModule.Config.ChangeUltCost > 0
                        ? ' ' + i18n.Get(
                            "prestige.dogstatue.cost",
                            new { cost = ProfessionsModule.Config.ChangeUltCost })
                        : string.Empty)),
            }).ToArray();
        }

        if (Skill.List.Any(s => GameLocation.canRespec(s)))
        {
            options = options.Concat(new Response[]
            {
                new(
                    "prestigeRespec",
                    i18n.Get("prestige.dogstatue.respec") +
                    (ProfessionsModule.Config.PrestigeRespecCost > 0
                        ? ' ' + i18n.Get(
                            "prestige.dogstatue.cost",
                            new { cost = ProfessionsModule.Config.PrestigeRespecCost })
                        : string.Empty)),
            }).ToArray();
        }

        location.createQuestionDialogue(message, options, "dogStatue");
    }

    #endregion dialog handlers
}
