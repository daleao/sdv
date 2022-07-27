namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using DaLion.Common;
using Events.GameLoop;
using Extensions;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationPerformActionPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string? action, Farmer who)
    {
        if (!ModEntry.Config.EnablePrestige || action?.StartsWith("DogStatue") != true || !who.IsLocalPlayer)
            return true; // run original logic

        try
        {
            string message;
            if (!ModEntry.Config.AllowPrestigeMultiplePerDay &&
                (ModEntry.EventManager.Get<PrestigeDayEndingEvent>()?.IsEnabled == true ||
                 ModEntry.State.UsedDogStatueToday))
            {
                message = ModEntry.i18n.Get("prestige.dogstatue.dismiss");
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (who.CanResetAnySkill())
            {
                message = ModEntry.i18n.Get("prestige.dogstatue.first");
                if (ModEntry.Config.ForgetRecipesOnSkillReset)
                    message += ModEntry.i18n.Get("prestige.dogstatue.forget");
                message += ModEntry.i18n.Get("prestige.dogstatue.offer");

                __instance.createQuestionDialogue(message, __instance.createYesNoResponses(), "dogStatue");
                return false; // don't run original logic
            }

            if (who.HasAllProfessions() && !ModEntry.State.UsedDogStatueToday)
            {
                message = ModEntry.i18n.Get("prestige.dogstatue.what");
                var options = Array.Empty<Response>();

                if (Game1.player.get_Ultimate() is not null)
                    options = options.Concat(new Response[]
                    {
                        new("changeUlt", ModEntry.i18n.Get("prestige.dogstatue.changeult") +
                                         (ModEntry.Config.ChangeUltCost > 0
                                             ? ' ' + ModEntry.i18n.Get("prestige.dogstatue.cost",
                                                 new {cost = ModEntry.Config.ChangeUltCost})
                                             : string.Empty))
                    }).ToArray();

                if (Skill.List.Any(s => GameLocation.canRespec(s)))
                    options = options.Concat(new Response[]
                    {
                        new("prestigeRespec",
                            ModEntry.i18n.Get("prestige.dogstatue.respec") +
                            (ModEntry.Config.PrestigeRespecCost > 0
                                ? ' ' + ModEntry.i18n.Get("prestige.dogstatue.cost",
                                    new {cost = ModEntry.Config.PrestigeRespecCost})
                                : string.Empty))
                    }).ToArray();

                __instance.createQuestionDialogue(message, options, "dogStatue");
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
}