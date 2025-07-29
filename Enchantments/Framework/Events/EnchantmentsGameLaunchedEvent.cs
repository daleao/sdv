namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Enchantments.Framework.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class EnchantmentsGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        SpaceCoreIntegration.Instance!.Register();
        if (EnchantmentsConfigMenu.Instance?.IsLoaded ?? false)
        {
            EnchantmentsConfigMenu.Instance.Register();
        }
    }
}
