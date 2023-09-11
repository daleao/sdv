namespace DaLion.Overhaul.Modules.Combat.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ValorWarpedEvent : WarpedEvent
{
    private static int _consecutiveFloorsVisited;

    /// <summary>Initializes a new instance of the <see cref="ValorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ValorWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.OldLocation is not MineShaft || e.NewLocation is not MineShaft)
        {
            return;
        }

        if (MineShaft.numberOfCraftedStairsUsedThisRun > 0)
        {
            _consecutiveFloorsVisited = 0;
        }

        _consecutiveFloorsVisited++;
        if (_consecutiveFloorsVisited < (CombatModule.Config.HeroQuestDifficulty == Config.Difficulty.Easy ? 50 : 100))
        {
            return;
        }

        e.Player.Write(DataKeys.ProvenValor, int.MaxValue.ToString());
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Valor);
        this.Disable();
    }
}
