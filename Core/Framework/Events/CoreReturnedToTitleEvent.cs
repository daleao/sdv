namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CoreReturnedToTitleEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreReturnedToTitleEvent(EventManager? manager = null)
    : ReturnedToTitleEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        this.Manager.Reset();
        PerScreenState.ResetAllScreens();
    }
}
