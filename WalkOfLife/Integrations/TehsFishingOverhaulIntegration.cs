using System;
using System.Linq.Expressions;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Tools;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Integrations;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Integrations
{
    internal class TehsFishingOverhaulIntegration : BaseIntegration
    {
        private static readonly Func<object, double> getTreasureBaseChance;
        private static readonly Func<object, double> getTreasurePirateFactor;

        private readonly ISimplifiedFishingApi _fishingApi;
        private readonly object _rawFishingApi;

        /// <summary>
        /// Lazily initializes the static getter fields. This is done lazily in case Teh's Fishing
        /// Overhaul isn't loaded and the types do not exist. By using expressions instead of
        /// reflection, we can avoid most of the overhead of dynamically accessing fields.
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
            Action<string, LogLevel> log) : base("Teh's Fishing Overhaul", "TehPers.FishingOverhaul", "3.2.0",
            modRegistry,
            log)
        {
            _fishingApi = GetValidatedApi<ISimplifiedFishingApi>();
            _rawFishingApi = modRegistry.GetApi("TehPers.FishingOverhaul");
        }

        public void Register()
        {
            // Fisher profession
            _fishingApi.ModifyChanceForFish(
                (who, chance) => who.CurrentTool is FishingRod rod &&
                                 Objects.BaitById.TryGetValue(rod.getBaitAttachmentIndex(), out var baitName)
                                 && baitName.IsAnyOf("Bait", "Wild Bait", "Magic Bait")
                                 && who.HasProfession("Fisher")
                    ? 1 - Math.Pow(1 - chance, 2.0)
                    : chance);

            // Remove effects of pirate
            _fishingApi.ModifyChanceForTreasure(
                (who, chance) => who.professions.Contains(9)
                    ? chance - getTreasureBaseChance(_rawFishingApi) * getTreasurePirateFactor(_rawFishingApi)
                    : chance);
        }
    }
}