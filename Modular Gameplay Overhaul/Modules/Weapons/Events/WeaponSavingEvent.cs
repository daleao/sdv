namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponSavingEvent : SavingEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponSavingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponSavingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection;

    /// <inheritdoc />
    protected override void OnSavingImpl(object? sender, SavingEventArgs e)
    {
        if (WeaponsModule.State.AutoSelectableWeapon is not null)
        {
            Game1.player.Write(
                DataKeys.SelectableWeapon,
                Game1.player.Items.IndexOf(WeaponsModule.State.AutoSelectableWeapon).ToString());
        }
    }
}
