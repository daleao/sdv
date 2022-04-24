namespace DaLion.Stardew.Prairie.Training.Framework.Events;

#region using directives

using System;
using JetBrains.Annotations;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

#endregion using directives

/// <summary>Wrapper for <see cref="IEvolutionAlgorithm{TGenome}.UpdateEvent"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class EvolutionAlgorithmUpdateEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.Network.UpdateEvent += OnUpdate;
        Log.D("[Prairie] Hooked EvolutionAlgorithmUpdate event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.Network.UpdateEvent -= OnUpdate;
        Log.D("[Prairie] Unhooked EvolutionAlgorithmUpdate event.");
    }

    /// <summary>Raised after every <see cref="UpdateScheme.logRate"/> generations.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnUpdate(object sender, EventArgs e)
    {
        var evolutionAlgorithm = (NeatEvolutionAlgorithm<NeatGenome>) sender;
        Log.D($" ****** Running generation {evolutionAlgorithm.CurrentGeneration} ****** \n" +
              $"Mean fitness: {evolutionAlgorithm.Statistics._meanFitness:N6}\n" +
              $"Best fitness: {evolutionAlgorithm.Statistics._maxFitness:N6}\n");
    }
}