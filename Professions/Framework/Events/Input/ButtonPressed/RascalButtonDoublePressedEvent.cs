namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RascalButtonDoublePressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class RascalButtonDoublePressedEvent(EventManager? manager = null)
    : ButtonDoublePressedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Rascal);

    /// <inheritdoc />
    protected override KeybindList Keybinds => Config.ModKey;

    /// <inheritdoc />
    protected override void OnButtonDoublePressedImpl()
    {
        var player = Game1.player;
        if (Game1.activeClickableMenu is not null ||
            player.CurrentTool is not Slingshot { AttachmentSlotsCount: 2 } slingshot ||
            slingshot.attachments.Length != 2 || slingshot.attachments[1] is null)
        {
            return;
        }

        if (slingshot.attachments[1] is { QualifiedItemId: QualifiedObjectIds.MonsterMusk })
        {
            Game1.playSound("cancel");
            return;
        }

        (slingshot.attachments[0], slingshot.attachments[1]) = (slingshot.attachments[1], slingshot.attachments[0]);
        Game1.playSound("button1");
    }
}
