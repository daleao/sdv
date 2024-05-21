namespace DaLion.Core.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CoreSaveLoadedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreSaveLoadedEvent(EventManager? manager = null)
    : SaveLoadedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        this.Manager.Enable<SlimeBallObjectListChangedEvent>();
    }
}
