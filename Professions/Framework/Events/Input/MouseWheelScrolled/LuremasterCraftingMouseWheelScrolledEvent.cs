namespace DaLion.Professions.Framework.Events.Input.MouseWheelScrolled;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterCraftingMouseWheelScrolledEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class LuremasterCraftingMouseWheelScrolledEvent(EventManager? manager = null)
    : MouseWheelScrolledEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.LuremasterCraftingRecipeBeingHovered is not null;

    /// <inheritdoc />
    protected override void OnMouseWheelScrolledImpl(object? sender, MouseWheelScrolledEventArgs e)
    {
        State.IsLuremasterUsingCursorInput = false;
        ModHelper.Input.SuppressScrollWheel();
        switch (e.Delta)
        {
            case > 0:
                State.LuremasterCraftingIngredientSelected--;
                break;
            case < 0:
                State.LuremasterCraftingIngredientSelected++;
                break;
        }
    }
}
