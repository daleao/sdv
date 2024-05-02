namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Linq.Expressions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.TehsFishingOverhaul;
using HarmonyLib;

#endregion using directives

[ModRequirement("TehPers.FishingOverhaul", "Teh's Fishing Overhaul", "3.2.0")]
internal sealed class TehsFishingOverhaulIntegration : ModIntegration<TehsFishingOverhaulIntegration, ISimplifiedFishingApi>
{
    private static readonly Func<object?, double> GetTreasureBaseChance;
    private static readonly Func<object?, double> GetTreasurePirateFactor;
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

    /// <summary>Initializes a new instance of the <see cref="TehsFishingOverhaulIntegration"/> class.</summary>
    internal TehsFishingOverhaulIntegration()
        : base(ModHelper.ModRegistry)
    {
        this._rawApi = this.ModRegistry.GetApi("TehPers.FishingOverhaul");
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        // remove Pirate perks
        this.ModApi.ModifyChanceForTreasure(
            (who, chance) => who.professions.Contains(9)
                ? chance - (GetTreasureBaseChance(this._rawApi) * GetTreasurePirateFactor(this._rawApi))
                : chance);
        Log.D("[PRFS]: Registered the Fishing Overhaul integration by Teh.");
        return true;
    }
}
