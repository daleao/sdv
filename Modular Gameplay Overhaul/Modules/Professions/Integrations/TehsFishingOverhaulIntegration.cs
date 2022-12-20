namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Collections.Generic;
using System.Linq.Expressions;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.TehsFishingOverhaul;
using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[RequiresMod("TehPers.FishingOverhaul", "Teh's Fishing Overhaul", "3.2.0")]
internal sealed class TehsFishingOverhaulIntegration : ModIntegration<TehsFishingOverhaulIntegration, ISimplifiedFishingApi>
{
    private static readonly Func<object?, double> GetTreasureBaseChance;

    private static readonly Func<object?, double> GetTreasurePirateFactor;

    // Mail flags added by TFO to track legendary fish progress
    private static readonly Dictionary<int, string> LegendaryFlags = new()
    {
        [159] = "TehPers.FishingOverhaul/crimsonfishCaught",
        [160] = "TehPers.FishingOverhaul/anglerCaught",
        [163] = "TehPers.FishingOverhaul/legendCaught",
        [682] = "TehPers.FishingOverhaul/mutantCarpCaught",
        [775] = "TehPers.FishingOverhaul/glacierfishCaught",
    };

    // Conversation topics added by [TFO] Recatchable Legendaries to track delays
    private static readonly List<string> RecatchableLegendariesTopics = new()
    {
        "TehPers.RecatchableLegendaries/crimsonfishDelay",
        "TehPers.RecatchableLegendaries/anglerDelay",
        "TehPers.RecatchableLegendaries/legendDelay",
        "TehPers.RecatchableLegendaries/mutantCarpDelay",
        "TehPers.RecatchableLegendaries/glacierfishDelay",
    };

    private readonly IModEvents _events;
    private readonly object? _rawApi;

    /// <summary>
    ///     Initializes static members of the <see cref="TehsFishingOverhaulIntegration"/> class.
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
        var getTreasureBaseChanceLazily = new Lazy<Func<object?, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var baseChanceProp = Expression.Property(treasureChancesProp, "BaseChance");
            return Expression.Lambda<Func<object?, double>>(baseChanceProp, simplifiedApiParam)
                .Compile();
        });

        var getTreasurePirateFactorLazily = new Lazy<Func<object?, double>>(() =>
        {
            var (simplifiedApiParam, treasureChancesProp) = commonExpressions.Value;
            var pirateFactorProp = Expression.Property(treasureChancesProp, "PirateFactor");
            return Expression.Lambda<Func<object?, double>>(pirateFactorProp, simplifiedApiParam)
                .Compile();
        });

        // Set the static fields
        GetTreasureBaseChance = fishingApi => getTreasureBaseChanceLazily.Value(fishingApi);
        GetTreasurePirateFactor = fishingApi => getTreasurePirateFactorLazily.Value(fishingApi);
    }

    private TehsFishingOverhaulIntegration()
        : base("TehPers.FishingOverhaul", "Teh's Fishing Overhaul", "3.2.0", ModHelper.ModRegistry)
    {
        this._events = ModHelper.Events;
        this._rawApi = this.ModRegistry.GetApi("TehPers.FishingOverhaul");
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        // add Fisher perks
        this.ModApi.ModifyChanceForFish(
            (who, chance) => who.CurrentTool is FishingRod rod &&
                             rod.getBaitAttachmentIndex() != 703 // magnet
                             && who.HasProfession(Profession.Fisher)
                ? 1 - Math.Pow(1 - chance, 2d)
                : chance);

        // remove Pirate perks
        this.ModApi.ModifyChanceForTreasure(
            (who, chance) => who.professions.Contains(9)
                ? chance - (GetTreasureBaseChance(this._rawApi) * GetTreasurePirateFactor(this._rawApi))
                : chance);

        // manage legendary caught flags
        bool? hadPrestigedAngler = null;
        this._events.GameLoop.UpdateTicking += (_, _) =>
        {
            // check the state of the prestiged angler profession
            var hasPrestigedAngler = Game1.player.HasProfession(Profession.Angler, true);
            switch (hadPrestigedAngler, hasPrestigedAngler)
            {
                // prestiged status was just lost
                case (not false, false):
                {
                    // Add flags for all legendary fish that have been caught
                    foreach (var (id, flag) in LegendaryFlags)
                    {
                        if (Game1.player.fishCaught.ContainsKey(id) && !Game1.player.mailReceived.Contains(flag))
                        {
                            Game1.player.mailReceived.Add(flag);
                        }
                    }

                    break;
                }

                // has the prestiged status
                case (_, true):
                {
                    // remove all legendary caught flags so they can be caught again
                    // note: does not remove the fish from the collections tab
                    foreach (var flag in LegendaryFlags.Values)
                    {
                        Game1.player.RemoveMail(flag);
                    }

                    // if Recatchable Legendaries is installed, reset the conversation topics
                    if (this.ModRegistry.IsLoaded("TehPers.RecatchableLegendaries"))
                    {
                        foreach (var topic in RecatchableLegendariesTopics)
                        {
                            Game1.player.activeDialogueEvents.Remove(topic);
                        }
                    }

                    break;
                }
            }

            // update previous state
            hadPrestigedAngler = hasPrestigedAngler;
        };

        return true;
    }
}
