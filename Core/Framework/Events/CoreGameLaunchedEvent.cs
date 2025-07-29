namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CoreGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class CoreGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        if (CoreConfigMenu.Instance?.IsLoaded ?? false)
        {
            CoreConfigMenu.Instance.Register();
        }
    }
}
