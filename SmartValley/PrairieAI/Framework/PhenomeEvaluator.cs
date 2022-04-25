namespace DaLion.Stardew.Prairie.Training.Framework;

#region using directives

using SharpNeat.Core;
using SharpNeat.Phenomes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Minigames;

using Common.Extensions.Reflection;

#endregion using directives

internal class PhenomeEvaluator : IPhenomeEvaluator<IBlackBox>
{
    private readonly int _killWeight;
    private readonly int _coinWeight;
    private readonly int _deathWeight;
    private readonly double _activationThreshold;

    public ulong EvaluationCount { get; private set; }
    public bool StopConditionSatisfied { get; private set; }

    public PhenomeEvaluator(int killWeight, int coinWeight, int deathWeight, double activationThreshold)
    {
        _killWeight = killWeight;
        _coinWeight = coinWeight;
        _deathWeight = deathWeight;
        _activationThreshold = activationThreshold;
    }

    /// <summary>Evaluate the provided <see cref="IBlackBox"/> against the Prairie King environment and return its fitness score.</summary>
    public FitnessInfo Evaluate(IBlackBox phenome)
    {
        ++EvaluationCount;
        phenome.InputSignalArray.Reset();

        if (!ModEntry.IsPlayingAbigailGame)
            Game1.game1.parseDebugInput("minigame cowboy");

        while (!StopConditionSatisfied)
        {
            var i = 0;
            foreach (var input in ModEntry.Inputs)
                phenome.InputSignalArray[i++] = (int) input;

            phenome.Activate();

            var j = 0;
            foreach (var key in ModEntry.Actions.Keys)
                ModEntry.Actions[key] = phenome.OutputSignalArray[j++] > _activationThreshold;

            StopConditionSatisfied = ModEntry.GameInstance is null || AbigailGame.gameOver || AbigailGame.waitingForPlayerToMoveDownAMap;
        }

        var fitness = _killWeight * ModEntry.EnemiesDefeated + _coinWeight * ModEntry.CoinsCollected - _deathWeight * ModEntry.DeathCount;
        return new(fitness, fitness);
    }

    /// <summary>Reset the internal state of the evaluation scheme if any exists.</summary>
    public void Reset()
    {
        if (ModEntry.IsPlayingAbigailGame) ModEntry.GameInstance.quit = true;

        ModEntry.EnemiesDefeated = 0;
        ModEntry.CoinsCollected = 0;
        ModEntry.DeathCount = 0;
    }
}