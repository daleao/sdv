namespace DaLion.Ligo.Modules.Professions.Patches.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Ligo.Modules.Professions.Events.GameLoop;
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
        if (!ModEntry.Config.Professions.EnablePrestige || action?.StartsWith("DogStatue") != true ||
            !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (!ModEntry.Config.Professions.AllowMultiplePrestige &&
                (ModEntry.Events.Get<PrestigeDayEndingEvent>()?.IsEnabled == true ||
                 ModEntry.State.Professions.UsedDogStatueToday))
            {
                message = ModEntry.i18n.Get("prestige.dogstatue.dismiss");
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

            message = ModEntry.i18n.Get("prestige.dogstatue.first");
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
        string message = ModEntry.i18n.Get("prestige.dogstatue.first");
        if (ModEntry.Config.Professions.ForgetRecipes)
        {
            message += ModEntry.i18n.Get("prestige.dogstatue.forget");
        }

        message += ModEntry.i18n.Get("prestige.dogstatue.offer");

        location.createQuestionDialogue(message, location.createYesNoResponses(), "dogStatue");
    }

    private static void OfferRespecOptions(GameLocation location)
    {
        string message = ModEntry.i18n.Get("prestige.dogstatue.what");
        var options = Array.Empty<Response>();

        if (Game1.player.Get_Ultimate() is not null)
        {
            options = options.Concat(new Response[]
            {
                new(
                    "changeUlt",
                    ModEntry.i18n.Get("prestige.dogstatue.changeult") +
                    (ModEntry.Config.Professions.ChangeUltCost > 0
                        ? ' ' + ModEntry.i18n.Get(
                            "prestige.dogstatue.cost",
                            new { cost = ModEntry.Config.Professions.ChangeUltCost })
                        : string.Empty)),
            }).ToArray();
        }

        if (Skill.List.Any(s => GameLocation.canRespec(s)))
        {
            options = options.Concat(new Response[]
            {
                new(
                    "prestigeRespec",
                    ModEntry.i18n.Get("prestige.dogstatue.respec") +
                    (ModEntry.Config.Professions.PrestigeRespecCost > 0
                        ? ' ' + ModEntry.i18n.Get(
                            "prestige.dogstatue.cost",
                            new { cost = ModEntry.Config.Professions.PrestigeRespecCost })
                        : string.Empty)),
            }).ToArray();
        }

        location.createQuestionDialogue(message, options, "dogStatue");
    }

    #endregion dialog handlers
}
