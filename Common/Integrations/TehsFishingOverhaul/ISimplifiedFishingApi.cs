using System;
using StardewValley;

namespace TheLion.Stardew.Common.Integrations;

public interface ISimplifiedFishingApi
{
    /// <summary>
    ///     Gets the chance that a fish would be caught. This does not take into account whether
    ///     there are actually fish to catch at the <see cref="Farmer" />'s location. If no fish
    ///     can be caught, then the <see cref="Farmer" /> will always catch trash.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer" /> catching the fish.</param>
    /// <returns>The chance a fish would be caught instead of trash.</returns>
    double GetChanceForFish(Farmer farmer);

    /// <summary>
    ///     Gets the chance that treasure will be found during the fishing minigame.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer" /> catching the treasure.</param>
    /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
    double GetChanceForTreasure(Farmer farmer);

    /// <summary>
    ///     Modifies the chance that a fish would be caught. The provided callback is invoked every
    ///     time <see cref="GetChanceForFish" /> is called to modify the resulting chance before
    ///     it's returned. Modifiers are invoked in the order they are registered.
    /// </summary>
    /// <param name="chanceModifier">
    ///     The chance modifier function. The input arguments are the <see cref="Farmer" /> and the
    ///     calculated chance, and the return value is the modified chance.
    /// </param>
    void ModifyChanceForFish(Func<Farmer, double, double> chanceModifier);

    /// <summary>
    ///     Modifies the chance that treasure would be found while fishing. The provided callback
    ///     is invoked every time <see cref="GetChanceForTreasure" /> is called to modify the
    ///     resulting chance before it's returned. Modifiers are invoked in the order they are
    ///     registered.
    /// </summary>
    /// <param name="chanceModifier">
    ///     The chance modifier function. The input arguments are the <see cref="Farmer" /> and the
    ///     calculated chance, and the return value is the modified chance.
    /// </param>
    void ModifyChanceForTreasure(Func<Farmer, double, double> chanceModifier);
}