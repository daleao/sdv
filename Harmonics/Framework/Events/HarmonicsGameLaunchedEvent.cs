namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Harmonics.Framework.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HarmonicsGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class HarmonicsGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        SpaceCoreIntegration.Instance!.Register();
        BetterCraftingIntegration.Instance?.Register();
        if (HarmonicsConfigMenu.Instance?.IsLoaded == true)
        {
            HarmonicsConfigMenu.Instance.Register();
        }
    }
}
