namespace DaLion.Ligo.Modules.Professions.Events.Content;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionAssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionAssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Refresh(e.NamesWithoutLocale.Where(name => name.IsEquivalentToAnyOf(
            $"{Manifest.UniqueID}/SkillBars",
            $"{Manifest.UniqueID}/UltimateMeter",
            $"{Manifest.UniqueID}/PrestigeProgression"))
            .ToImmutableHashSet());
    }
}
