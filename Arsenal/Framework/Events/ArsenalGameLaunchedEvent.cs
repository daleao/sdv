namespace DaLion.Arsenal.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ArsenalGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ArsenalGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? ArsenalMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        if (ArsenalConfigMenu.Instance?.IsLoaded == true)
        {
            ArsenalConfigMenu.Instance.Register();
        }
    }
}
