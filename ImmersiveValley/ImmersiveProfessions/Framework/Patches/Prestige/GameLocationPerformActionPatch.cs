namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatch"/> class.</summary>
    internal GameLocationPerformActionPatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ModEntry.Config.EnablePrestige || action?.StartsWith("DogStatue") != true || !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (!ModEntry.Config.AllowMultiplePrestige &&
                (ModEntry.Events.Get<PrestigeDayEndingEvent>()?.IsEnabled == true ||
                 ModEntry.State.UsedDogStatueToday))
            {
                message = ModEntry.i18n.Get("prestige.dogstatue.dismiss");
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (who.CanResetAnySkill())
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
        if (ModEntry.Config.ForgetRecipes)
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
                    (ModEntry.Config.ChangeUltCost > 0
                        ? ' ' + ModEntry.i18n.Get("prestige.dogstatue.cost", new { cost = ModEntry.Config.ChangeUltCost })
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
                    (ModEntry.Config.PrestigeRespecCost > 0
                        ? ' ' + ModEntry.i18n.Get("prestige.dogstatue.cost", new { cost = ModEntry.Config.PrestigeRespecCost })
                        : string.Empty)),
            }).ToArray();
        }

        location.createQuestionDialogue(message, options, "dogStatue");
    }

    #endregion dialog handlers
}
