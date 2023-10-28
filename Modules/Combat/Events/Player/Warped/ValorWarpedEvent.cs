namespace DaLion.Overhaul.Modules.Combat.Events.Player.Warped;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class ValorWarpedEvent : WarpedEvent
{
    private static int _consecutiveFloorsVisited;

    /// <summary>Initializes a new instance of the <see cref="ValorWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ValorWarpedEvent(EventManager manager)
        : base(manager)
    {
        this.Enable();
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.State.HeroQuest is not null;

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.OldLocation is not MineShaft || e.NewLocation is not MineShaft shaft)
        {
            return;
        }

        if (shaft.GetAdditionalDifficulty() < 1)
        {
            return;
        }

        if (MineShaft.numberOfCraftedStairsUsedThisRun > 0)
        {
            _consecutiveFloorsVisited = 0;
        }

        _consecutiveFloorsVisited++;
        var objective = CombatModule.Config.HeroQuestDifficulty == CombatConfig.QuestDifficulty.Easy
            ? 10
            : CombatModule.Config.HeroQuestDifficulty == CombatConfig.QuestDifficulty.Medium
                ? 20
                : 40;
        if (_consecutiveFloorsVisited < objective)
        {
            return;
        }

        e.Player.Write(Virtue.Valor.Name, int.MaxValue.ToString());
        Game1.chatBox.addMessage(I18n.Virtues_Recognize_Yoba(), Color.Green);
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Valor);
        this.Disable();
    }
}
