namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RascalButtonDoublePressedEvent"/> class.</summary>
[UsedImplicitly]
internal sealed class RascalButtonDoublePressedEvent : ButtonDoublePressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RascalButtonDoublePressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    public RascalButtonDoublePressedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
        this.OnButtonDoublePressed = this.OnButtonDoublePressedImpl;
    }

    /// <inheritdoc />
    public override KeybindList KeybindList => Config.ModKey;

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Rascal);

    /// <inheritdoc />
    public override void OnButtonDoublePressedImpl()
    {
        var player = Game1.player;
        if (Game1.activeClickableMenu is not null ||
            player.CurrentTool is not Slingshot { AttachmentSlotsCount: 2 } slingshot ||
            slingshot.attachments.Length != 2 || slingshot.attachments[1] is null)
        {
            return;
        }

        if (slingshot.attachments[1] is { QualifiedItemId: QIDs.MonsterMusk })
        {
            Game1.playSound("cancel");
            return;
        }

        (slingshot.attachments[0], slingshot.attachments[1]) = (slingshot.attachments[1], slingshot.attachments[0]);
        Game1.playSound("button1");
    }
}
