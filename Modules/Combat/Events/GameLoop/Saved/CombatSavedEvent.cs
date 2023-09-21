namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.Saved;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.Saving;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatSavedEvent : SavedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatSavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatSavedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => CombatModule.Config.EnableWeaponOverhaul || CombatModule.Config.EnableHeroQuest;

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
    {
        CombatSavingEvent.InstrinsicWeapons.ForEach(weapon => weapon.AddIntrinsicEnchantments());
        CombatSavingEvent.InstrinsicWeapons.Clear();
    }
}
