namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Professions.Framework.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using xTile.Dimensions;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationPerformActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.performAction),
            [typeof(string[]), typeof(Farmer), typeof(Location)]);
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty into Statue of Transcendance.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.VeryHigh)]
    [UsedImplicitly]
    private static bool GameLocationPerformActionPrefix(GameLocation __instance, ref bool __result, string[] action, Farmer who, Location tileLocation)
    {
        if (__instance.ShouldIgnoreAction(action, who, tileLocation) || !ArgUtility.TryGet(action, 0, out var actionType, out _, allowBlank: true, "string actionType") || !who.IsLocalPlayer)
        {
            return true; // run original logic
        }

        if (actionType is "Mine" or "SkullDoor" && who.HasProfession(Profession.Spelunker, true) && State.SpelunkerFlag is not null)
        {
            switch (actionType)
            {
                case "Mine":
                    if (!ArgUtility.TryGetOptionalInt(action, 1, out var mineLevel, out _, 1, "int mineLevel"))
                    {
                        return true; // run original logic
                    }

                    __instance.playSound("stairsdown");
                    Game1.enterMine(State.SpelunkerFlagLevel);
                    break;
                case "SkullDoor":
                    if (!who.hasSkullKey || !who.hasUnlockedSkullDoor || Utility.IsPassiveFestivalDay("DesertFestival"))
                    {
                        return true; // run original logic
                    }
                    else if (who.secretNotesSeen.Contains(10) && !who.mailReceived.Contains("qiCave"))
                    {
                        return true; // run original logic
                    }
                    else
                    {
                        who.completelyStopAnimatingOrDoingAction();
                        __instance.playSound("doorClose");
                        DelayedAction.playSoundAfterDelay("stairsdown", 500, __instance);
                        Game1.enterMine(State.SpelunkerFlagLevel);
                        MineShaft.numberOfCraftedStairsUsedThisRun = 0;
                    }

                    break;
            }

            __result = true;
            return false; // don't run original logic
        }

        if (!ShouldEnableSkillReset && !ShouldEnablePrestigeLevels && !ShouldEnableLimitBreaks)
        {
            return true; // run original logic
        }

        try
        {
            if (actionType == "MasteryRoom")
            {
                var count = Skill.List.Count(s => s.CurrentLevel >= 10);
                if (count >= 5)
                {
                    Game1.playSound("doorClose");
                    Game1.warpFarmer("MasteryCave", 7, 11, 0);
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\1_6_Strings:MasteryCave", count));
                }

                __result = true;
                return false; // don't run original logic
            }

            if (!actionType.Contains("DogStatue"))
            {
                return true; // run original logic
            }

            string message;
            if (ShouldEnableSkillReset)
            {
                if (!Config.Skills.AllowMultipleResets && State.SkillsToReset.Count > 0)
                {
                    message = I18n.Prestige_DogStatue_Dismiss();
                    Game1.drawObjectDialogue(message);
                    __result = true;
                    return false; // don't run original logic
                }

                if (TryOfferSkillReset(__instance))
                {
                    __result = true;
                    return false; // don't run original logic
                }
            }

            if ((ShouldEnablePrestigeLevels || ShouldEnableLimitBreaks) && TryOfferRespecOptions(__instance))
            {
                __result = true;
                return false; // don't run original logic
            }

            if (!ShouldEnableSkillReset)
            {
                return true; // run original logic
            }

            message = I18n.Prestige_DogStatue_First();
            Game1.drawObjectDialogue(message);
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

        if (ShouldEnablePrestigeLevels && Skill.List.Any(s => location.CanRespecPrestiged(s)))
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
