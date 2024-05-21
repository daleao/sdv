namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        foreach (var machine in Config.ArtisanMachines)
        {
            Lookups.ArtisanMachines.Add(machine);
        }

        foreach (var good in Config.AnimalDerivedGoods)
        {
            Lookups.AnimalDerivedGoods.Add(good);
        }

        if (Config.BeesAreAnimals)
        {
            Lookups.AnimalDerivedGoods.Add(QualifiedObjectIds.Honey);
            Lookups.AnimalDerivedGoods.Add(QualifiedObjectIds.Mead);
        }
    }
}
