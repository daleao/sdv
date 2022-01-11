using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Prestige;

[UsedImplicitly]
internal class GameLocationPerformActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationPerformActionPatch()
    {
        Original = RequireMethod<GameLocation>(nameof(GameLocation.performAction));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Prestige.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string action, Farmer who)
    {
        if (!ModEntry.Config.EnablePrestige || action is null || action.Split(' ')[0] != "DogStatue" ||
            !who.IsLocalPlayer)
            return true; // run original logic

        try
        {
            string message;
            if (!ModEntry.Config.AllowPrestigeMultiplePerDay &&
                (ModEntry.EventManager.Get<PrestigeDayEndingEvent>().IsEnabled ||
                 ModEntry.State.Value.UsedDogStatueToday))
            {
                message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.dismiss");
                Game1.drawObjectDialogue(message);
                return false; // don't run original logic
            }

            if (who.CanResetAnySkill())
            {
                message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first");
                if (ModEntry.Config.ForgetRecipesOnSkillReset)
                    message += ModEntry.ModHelper.Translation.Get("prestige.dogstatue.forget");
                message += ModEntry.ModHelper.Translation.Get("prestige.dogstatue.offer");

                __instance.createQuestionDialogue(message, __instance.createYesNoResponses(), "dogStatue");
                return false; // don't run original logic
            }

            if (who.HasAllProfessions() && !ModEntry.State.Value.UsedDogStatueToday)
            {
                message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.what");
                Response[] options = Array.Empty<Response>();

                if (ModEntry.State.Value.SuperMode is not null)
                    options = options.Concat(new Response[]
                    {
                        new("changeUlt", ModEntry.ModHelper.Translation.Get("prestige.dogstatue.changeult") +
                                         (ModEntry.Config.ChangeUltCost > 0
                                             ? ' ' + ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost",
                                                 new {cost = ModEntry.Config.ChangeUltCost})
                                             : string.Empty))
                    }).ToArray();

                if (Enum.GetValues<SkillType>().Any(s => GameLocation.canRespec((int) s)))
                    options = options.Concat(new Response[]
                    {
                        new("prestigeRespec",
                            ModEntry.ModHelper.Translation.Get("prestige.dogstatue.respec") +
                            (ModEntry.Config.PrestigeRespecCost > 0
                                ? ' ' + ModEntry.ModHelper.Translation.Get("prestige.dogstatue.cost",
                                    new {cost = ModEntry.Config.PrestigeRespecCost})
                                : string.Empty))
                    }).ToArray();

                __instance.createQuestionDialogue(message, options, "dogStatue");
                return false; // don't run original logic
            }

            message = ModEntry.ModHelper.Translation.Get("prestige.dogstatue.first");
            Game1.drawObjectDialogue(message);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}