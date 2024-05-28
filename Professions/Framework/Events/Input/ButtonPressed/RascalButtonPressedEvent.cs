namespace DaLion.Professions.Framework.Events.Input.ButtonPressed;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RascalButtonPressedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class RascalButtonPressedEvent(EventManager? manager = null)
    : ButtonPressedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Rascal);

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Config.ModKey.JustPressed())
        {
            return;
        }

        var player = Game1.player;
        if (Game1.activeClickableMenu is not null || player.CurrentTool is not Slingshot slingshot ||
            slingshot.AttachmentSlotsCount != 2 || slingshot.attachments.Length != 2)
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

        // !!! COMBAT INTERVENTION NEEDED
    }
}
