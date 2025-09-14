namespace DaLion.Professions.Framework.Events.Input.CursorMoved;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperVisionCursorMovedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class PiperVisionCursorMovedEvent(EventManager? manager = null)
    : CursorMovedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static readonly List<GreenSlime> _SlimesHere = [];

    internal static GreenSlime? SlimeBeingHovered { get; private set; } = null;

    protected override void OnEnabled()
    {
        _SlimesHere.Clear();
        foreach (var slime in Game1.currentLocation.characters.OfType<GreenSlime>())
        {
            _SlimesHere.Add(slime);
        }

        if (!_SlimesHere.Any())
        {
            this.Disable();
        }
    }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        var (x, y) = e.NewPosition.AbsolutePixels;
        GreenSlime? hovered = null;
        foreach (var slime in _SlimesHere)
        {
            var bbox = slime.GetBoundingBox();
            bbox.Y -= bbox.Height;
            bbox.Height *= 2;
            if (!bbox.Contains(x, y))
            {
                continue;
            }

            hovered = slime;
            goto setHovered;
        }

        hovered = null;

    setHovered:
        if (SlimeBeingHovered != hovered)
        {
            SlimeBeingHovered = hovered;
        }
    }
}
