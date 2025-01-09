namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DemolitionistButtonDoublePressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class DemolitionistButtonDoublePressedEvent(EventManager? manager = null)
    : ButtonDoublePressedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Demolitionist);

    /// <inheritdoc />
    protected override KeybindList Keybinds => Config.ModKey;

    /// <inheritdoc />
    protected override void OnButtonDoublePressedImpl()
    {
        if (Game1.activeClickableMenu is not null || Game1.player.ActiveItem?.QualifiedItemId is not
            (QualifiedObjectIds.CherryBomb or QualifiedObjectIds.Bomb or QualifiedObjectIds.MegaBomb))
        {
            return;
        }

        State.IsManualDetonationModeEnabled = !State.IsManualDetonationModeEnabled;
        Game1.playSound("button1");
    }
}
