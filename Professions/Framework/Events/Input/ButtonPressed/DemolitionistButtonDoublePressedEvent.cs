namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DemolitionistButtonDoublePressedEvent"/> class.</summary>
[UsedImplicitly]
[ImplicitIgnore]
internal sealed class DemolitionistButtonDoublePressedEvent : ButtonDoublePressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DemolitionistButtonDoublePressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    public DemolitionistButtonDoublePressedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this.OnButtonDoublePressed = this.OnButtonDoublePressedImpl;
    }

    /// <inheritdoc />
    public override KeybindList KeybindList => Config.ModKey;

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Demolitionist);

    /// <inheritdoc />
    public override void OnButtonDoublePressedImpl()
    {
        if (Game1.activeClickableMenu is not null || Game1.player.ActiveItem?.QualifiedItemId is not
                (QIDs.CherryBomb or QIDs.Bomb or QIDs.MegaBomb))
        {
            return;
        }

        State.IsManualDetonationModeEnabled = !State.IsManualDetonationModeEnabled;
        Game1.playSound("button1");
        if (State.IsManualDetonationModeEnabled)
        {
            Game1.addHUDMessage(new HUDMessage(I18n.Demolitionist_Manual()));
            Log.D("Manual detonation mode engaged.");
        }
        else
        {
            Game1.addHUDMessage(new HUDMessage(I18n.Demolitionist_Timed()));
            Log.D("Manual detonation mode disengaged.");
        }
    }
}
