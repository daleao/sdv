namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

using Common.Integrations;
using Framework.Extensions;
using Framework.Utility;

#endregion using directives

internal class TehsFishingOverhaulIntegration : BaseIntegration
{
    // Mail flags added by TFO to track legendary fish progress
    private static readonly Dictionary<int, string> legendaryFlags = new()
    {
        [159] = "TehPers.FishingOverhaul/crimsonfishCaught",
        [160] = "TehPers.FishingOverhaul/anglerCaught",
        [163] = "TehPers.FishingOverhaul/legendCaught",
        [682] = "TehPers.FishingOverhaul/mutantCarpCaught",
        [775] = "TehPers.FishingOverhaul/glacierfishCaught"
    };

    // Conversation topics added by [TFO] Recatchable Legendaries to track delays
    private static readonly List<string> recatchableLegendariesTopics = new()
    {
        "TehPers.RecatchableLegendaries/crimsonfishDelay",
        "TehPers.RecatchableLegendaries/anglerDelay",
        "TehPers.RecatchableLegendaries/legendDelay",
        "TehPers.RecatchableLegendaries/mutantCarpDelay",
        "TehPers.RecatchableLegendaries/glacierfishDelay"
    };

    private static readonly Func<object, double> getTreasureBaseChance;
    private static readonly Func<object, double> getTreasurePirateFactor;

    private readonly ISimplifiedFishingApi _fishingApi;
    private readonly IModHelper _helper;
    private readonly object _rawFishingApi;

    /// <summary>
    ///     Lazily initializes the static getter fields. This is done lazily in case Teh's Fishing
    ///     Overhaul isn't loaded and the types do not exist. By using expressions instead of
    ///     reflection, we can avoid most of the overhead of dynamically accessing fields.
    /// </summary>
    static TehsFishingOverhaulIntegration()
    {
        // Lazily create common expressions
        var commonExpressions = new Lazy<(ParameterExpression, Expression)>(() =>
        {
            var fishingApiType = AccessTools.TypeByName("TehPers.FishingOverhaul.Services.FishingApi");

            var simplifiedApiParam = Expression.Parameter(typeof(object), "fishingApi");
            var castedApi = Expression.Convert(simplifiedApiParam, fishingApiType);
            var treasureConfigField = Expression.Field(castedApi, "treasureConfig");
            var treasureChancesProp = Expression.Property(treasureConfigField, "TreasureChances");

            return (simplifiedApiParam, treasureChancesProp);
        });

        // Lazily create expressions
        var getTreasureBaseChanceLazily = new Lazy<Func<object, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var baseChanceProp = Expression.Property(treasureChancesProp, "BaseChance");
            return Expression.Lambda<Func<object, double>>(baseChanceProp, simplifiedApiParam)
                .Compile();
        });
        var getTreasurePirateFactorLazily = new Lazy<Func<object, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var pirateFactorProp = Expression.Property(treasureChancesProp, "PirateFactor");
            return Expression.Lambda<Func<object, double>>(pirateFactorProp, simplifiedApiParam)
                .Compile();
        });

        // Set the static fields
        getTreasureBaseChance = fishingApi => getTreasureBaseChanceLazily.Value(fishingApi);
        getTreasurePirateFactor = fishingApi => getTreasurePirateFactorLazily.Value(fishingApi);
    }

    public TehsFishingOverhaulIntegration(
        IModRegistry modRegistry,
        Action<string, LogLevel> log,
        IModHelper helper
    ) : base("Teh's Fishing Overhaul", "TehPers.FishingOverhaul", "3.2.0",
        modRegistry,
        log)
    {
        _helper = helper;
        _fishingApi = GetValidatedApi<ISimplifiedFishingApi>();
        _rawFishingApi = modRegistry.GetApi("TehPers.FishingOverhaul");
    }

    public void Register()
    {
        // add Fisher perks
        _fishingApi.ModifyChanceForFish(
            (who, chance) => who.CurrentTool is FishingRod rod &&
                             ObjectLookups.BaitById.TryGetValue(rod.getBaitAttachmentIndex(), out var baitName)
                             && baitName != "Magnet"
                             && who.HasProfession("Fisher")
                ? 1 - Math.Pow(1 - chance, 2.0)
                : chance);

        // remove Pirate perks
        _fishingApi.ModifyChanceForTreasure(
            (who, chance) => who.professions.Contains(9)
                ? chance - getTreasureBaseChance(_rawFishingApi) * getTreasurePirateFactor(_rawFishingApi)
                : chance);

        // manage legendary caught flags
        bool? hadPrestigedAngler = null;
        _helper.Events.GameLoop.UpdateTicking += (_, _) =>
        {
            // check the state of the prestiged angler profession
            var hasPrestigedAngler = Game1.player.HasPrestigedProfession("Angler");
            switch (hadPrestigedAngler, hasPrestigedAngler)
            {
                // prestiged status was just lost
                case (not false, false):
                {
                    // Add flags for all legendary fish that have been caught
                    foreach (var (id, flag) in legendaryFlags)
                        if (Game1.player.fishCaught.ContainsKey(id) && !Game1.player.mailReceived.Contains(flag))
                            Game1.player.mailReceived.Add(flag);

                    break;
                }

                // has the prestiged status
                case (_, true):
                {
                    // remove all legendary caught flags so they can be caught again
                    // note: does not remove the fish from the collections tab
                    foreach (var flag in legendaryFlags.Values) Game1.player.RemoveMail(flag);

                    // if Recatchable Legendaries is installed, reset the conversation topics
                    if (_helper.ModRegistry.IsLoaded("TehPers.RecatchableLegendaries"))
                        foreach (var topic in recatchableLegendariesTopics)
                            Game1.player.activeDialogueEvents.Remove(topic);

                    break;
                }
            }

            // update previous state
            hadPrestigedAngler = hasPrestigedAngler;
        };
    }
}