namespace DaLion.Redux.Framework.Professions.Events.Content;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabled]
internal sealed class StaticAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticAssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StaticAssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Refresh(e.NamesWithoutLocale.Where(name => name.IsEquivalentToAnyOf(
            $"{ModEntry.Manifest.UniqueID}/SkillBars",
            $"{ModEntry.Manifest.UniqueID}/UltimateMeter",
            $"{ModEntry.Manifest.UniqueID}/PrestigeProgression"))
            .ToImmutableHashSet());
    }
}
