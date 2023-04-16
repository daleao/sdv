namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Overhaul.Modules.Weapons;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        if (Data.WeaponRevalidationState.TryGetValue(player.Name + '/' + player.farmName.Value, out var state) &&
            WeaponsModule.ShouldEnable == state)
        {
            return;
        }

        Utils.RevalidateAllWeapons();
        Utils.RefreshAllWeapons(RefreshOption.Initial);
        Data.WeaponRevalidationState[player.Name + '/' + player.farmName.Value] = WeaponsModule.ShouldEnable;
        ModHelper.Data.WriteJsonFile("data.json", Data);
    }
}
