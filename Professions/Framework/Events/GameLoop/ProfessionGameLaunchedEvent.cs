namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProfessionGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        Lookups.ArtisanMachines.UnionWith(ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_ArtisanMachines")["ArtisanMachines"]);
        Lookups.AnimalDerivedGoods.UnionWith(ModHelper.GameContent.Load<Dictionary<string, string[]>>($"{UniqueId}_AnimalDerivedGoods")["AnimalDerivedGoods"]);
        if (!Config.BeesAreAnimals)
        {
            return;
        }

        Lookups.AnimalDerivedGoods.Add(QIDs.Honey);
        Lookups.AnimalDerivedGoods.Add(QIDs.Mead);
    }
}
