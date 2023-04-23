namespace DaLion.Overhaul.Modules.Core.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CoreSavedEvent : SavedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CoreSavedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CoreSavedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSavedImpl(object? sender, SavedEventArgs e)
    {
        var data = Config.ToString() + Manifest.Version;
        var hash = data.GetDeterministicHashCode();
        Game1.player.Write("checksum", hash.ToString());
    }
}
