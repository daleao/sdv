namespace DaLion.Professions.Framework.Events.Input.CursorMoved;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TapperCraftingCursorMovedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[LimitEvent]
internal sealed class TapperCraftingCursorMovedEvent(EventManager? manager = null)
    : CursorMovedEvent(manager ?? ProfessionsMod.EventManager)
{
    private float _yPositionSinceLastScroll;

    /// <inheritdoc />
    public override bool IsEnabled => State.TapperCraftingRecipeBeingHovered is not null;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._yPositionSinceLastScroll = -1f;
    }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        if (this._yPositionSinceLastScroll < 0)
        {
            this._yPositionSinceLastScroll = e.OldPosition.ScreenPixels.Y;
            return;
        }

        var verticalMovement = this._yPositionSinceLastScroll - e.NewPosition.ScreenPixels.Y;
        if (Math.Abs(verticalMovement) > 64)
        {
            if (verticalMovement > 0)
            {
                State.TapperCraftingIngredientSelected--;
            }
            else
            {
                State.TapperCraftingIngredientSelected++;
            }

            this._yPositionSinceLastScroll = e.NewPosition.ScreenPixels.Y;
        }
    }
}
