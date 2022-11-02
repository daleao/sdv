namespace DaLion.Redux.Framework.Arsenal.Weapons.Events;

#region using directives

using DaLion.Redux.Framework.Arsenal.Weapons.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalGameLaunchedEvent(EventManager manager)
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
