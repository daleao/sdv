namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using DaLion.Overhaul.Modules.Rings.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class RingGameLaunchedEvent : GameLaunchedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RingGameLaunchedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RingGameLaunchedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        // only soft dependencies
        BetterCraftingIntegration.Instance?.Register();
        WearMoreRingsIntegration.Instance?.Register();
        BetterRingsIntegration.Instance?.Register();

        // these two are mutually exclusive
        if (BetterRingsIntegration.Instance?.IsRegistered != true)
        {
            VanillaTweaksIntegration.Instance?.Register();
        }

        JsonAssetsIntegration.Instance?.Register();
    }
}
