namespace DaLion.Ligo.Modules.Arsenal.Weapons.Events;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Weapons.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        var registry = ModEntry.ModHelper.ModRegistry;

        // add Hero Soul item
        new DynamicGameAssetsIntegration(registry).Register();

        // register new enchantments
        new SpaceCoreIntegration(registry).Register();
    }
}
