namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HarmonicsReturnedToTitleEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class HarmonicsReturnedToTitleEvent(EventManager? manager = null)
    : ReturnedToTitleEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        PerScreenState.ResetAllScreens();
    }
}
