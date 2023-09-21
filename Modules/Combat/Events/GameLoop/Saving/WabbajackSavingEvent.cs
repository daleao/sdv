namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.Saving;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WabbajackSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="WabbajackSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WabbajackSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        foreach (var monster in WabbajackEnchantment.TransfiguredMonsters)
        {
            monster.currentLocation.characters.Remove(monster);
        }

        WabbajackEnchantment.TransfiguredMonsters.Clear();

        foreach (var location in Game1.locations)
        {
            location.Get_Animals().Clear();
        }
    }
}
