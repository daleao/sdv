namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="LuremasterCraftingButtonPressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class LuremasterCraftingButtonPressedEvent(EventManager? manager = null)
    : ButtonPressedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.LuremasterCraftingRecipeBeingHovered is not null;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        State.IsLuremasterUsingCursorInput = false;
        switch (e.Button)
        {
            case SButton.DPadUp:
            case SButton.Up:
                State.LuremasterCraftingIngredientSelected -= 10;
                break;
            case SButton.DPadRight:
            case SButton.Right:
                State.LuremasterCraftingIngredientSelected++;
                break;
            case SButton.DPadDown:
            case SButton.Down:
                State.LuremasterCraftingIngredientSelected += 10;
                break;
            case SButton.DPadLeft:
            case SButton.Left:
                State.LuremasterCraftingIngredientSelected--;
                break;
        }
    }
}
