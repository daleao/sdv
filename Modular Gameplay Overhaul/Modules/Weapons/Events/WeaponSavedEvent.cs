namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponSavedEvent : SavedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSavedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => WeaponsModule.Config.EnableRebalance || WeaponsModule.Config.InfinityPlusOne;

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
    {
        WeaponSavingEvent.InstrinsicWeapons.ForEach(weapon => weapon.AddIntrinsicEnchantments());
        WeaponSavingEvent.InstrinsicWeapons.Clear();
    }
}
