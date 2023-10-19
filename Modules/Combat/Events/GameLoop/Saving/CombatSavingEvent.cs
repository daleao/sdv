namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.Saving;

#region using directives

using DaLion.Overhaul.Modules.Combat;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        if (CombatModule.State.AutoSelectableMelee is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableMelee,
                Game1.player.Items.IndexOf(CombatModule.State.AutoSelectableMelee).ToString());
        }

        if (CombatModule.State.AutoSelectableRanged is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableRanged,
                Game1.player.Items.IndexOf(CombatModule.State.AutoSelectableRanged).ToString());
        }
    }
}
