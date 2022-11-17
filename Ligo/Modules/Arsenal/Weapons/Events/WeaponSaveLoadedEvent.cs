namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class WeaponSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (ModEntry.Config.Arsenal.Weapons.RebalanceWeapons)
        {
            Utils.UpdateAllWeapons();
        }
        else if (ModEntry.Config.Arsenal.Weapons.BringBackStabbySwords)
        {
            Utils.ConvertAllStabbySwords();
        }
    }
}
