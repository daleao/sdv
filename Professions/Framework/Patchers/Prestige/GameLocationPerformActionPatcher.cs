namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using xTile.Dimensions;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationPerformActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.performAction),
            [typeof(string[]), typeof(Farmer), typeof(Location)]);
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Masteries.</summary>
    [HarmonyPrefix]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, string[] action, Farmer who, Location tileLocation)
    {
        if ((!ShouldEnableSkillReset && !ShouldEnablePrestigeLevels && !ShouldEnableLimitBreaks) ||
            __instance.ShouldIgnoreAction(action, who, tileLocation) ||
            !ArgUtility.TryGet(action, 0, out var actionType, out _) || !actionType.Contains("DogStatue") ||
            !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        try
        {
            string message;
            if (ShouldEnableSkillReset)
            {
                if (!Config.Skills.AllowMultipleResets && State.SkillsToReset.Count > 0)
                {
                    message = I18n.Prestige_DogStatue_Dismiss();
                    Game1.drawObjectDialogue(message);
                    return false; // don't run original logic
                }

                if (TryOfferSkillReset(__instance))
                {
                    return false; // don't run original logic
                }
            }

            if ((ShouldEnablePrestigeLevels || ShouldEnableLimitBreaks) && TryOfferRespecOptions(__instance))
            {
                return false; // don't run original logic
            }

            if (!ShouldEnableSkillReset)
            {
                return true; // run original logic
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

    private static bool TryOfferSkillReset(GameLocation location)
    {
        if (!ISkill.CanResetAny())
        {
            return false;
        }

        var message = I18n.Prestige_DogStatue_First();
        if (Config.Skills.ForgetRecipesOnSkillReset)
        {
            message += I18n.Prestige_DogStatue_Forget();
        }

        message += I18n.Prestige_DogStatue_Offer();
        location.createQuestionDialogue(message, location.createYesNoResponses(), "dogStatue");
        return true;
    }

    private static bool TryOfferRespecOptions(GameLocation location)
    {
        var message = ShouldEnableSkillReset
            ? I18n.Prestige_DogStatue_Transcendance()
            : I18n.Prestige_DogStatue_Vanilla();
        var options = Array.Empty<Response>();

        if (ShouldEnableLimitBreaks && Skill.Combat.CanGainPrestigeLevels() &&
            Game1.player.professions.Intersect(((ISkill)Skill.Combat).TierTwoProfessionIds).Count() is var count &&
            (count > 1 || (count == 1 && State.LimitBreak is null)))
        {
            options =
            [
                .. options,
                .. new Response[]
                {
                    new(
                        "changeUlt",
                        I18n.Prestige_DogStatue_Change() +
                        (State.LimitBreak is not null && Config.Masteries.LimitRespecCost > 0
                            ? ' ' + I18n.Prestige_DogStatue_Cost(Config.Masteries.LimitRespecCost)
                            : string.Empty)),
                },
            ];
        }

        if (ShouldEnablePrestigeLevels && Skill.List.Any(s => GameLocation.canRespec(s)))
        {
            options =
            [
                .. options,
                .. new Response[]
                {
                    new(
                        "prestigeRespec",
                        I18n.Prestige_DogStatue_Respec() +
                        (Config.Masteries.PrestigeRespecCost > 0
                            ? ' ' + I18n.Prestige_DogStatue_Cost(Config.Masteries.PrestigeRespecCost)
                            : string.Empty)),
                },
            ];
        }

        if (options.Length <= 0)
        {
            return false;
        }

        location.createQuestionDialogue(message, options, "dogStatue");
        return true;
    }

    #endregion dialog handlers
}
