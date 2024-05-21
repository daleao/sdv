namespace DaLion.Professions.Framework.Events.Content;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionAssetsInvalidatedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionAssetsInvalidatedEvent(EventManager? manager = null)
    : AssetsInvalidatedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Reload(e.NamesWithoutLocale.AsEnumerable());
    }
}
