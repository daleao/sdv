namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="TapperCraftingButtonPressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class TapperCraftingButtonPressedEvent(EventManager? manager = null)
    : ButtonPressedEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.TapperCraftingRecipeBeingHovered is not null;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        switch (e.Button)
        {
            case SButton.DPadUp:
            case SButton.Up:
                ModHelper.Input.Suppress(e.Button);
                State.TapperCraftingIngredientSelected--;
                break;
            case SButton.DPadDown:
            case SButton.Down:
                ModHelper.Input.Suppress(e.Button);
                State.TapperCraftingIngredientSelected++;
                break;
        }

    }
}
