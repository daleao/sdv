namespace DaLion.Overhaul.Modules.Professions.Events.Input.ButtonPressed;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RascalButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="RascalButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal RascalButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Rascal);

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!ProfessionsModule.Config.ModKey.JustPressed())
        {
            return;
        }

        var player = Game1.player;
        if (Game1.activeClickableMenu is not null || player.CurrentTool is not Slingshot slingshot ||
            slingshot.numAttachmentSlots.Value < 2 || slingshot.attachments.Count < 2)
        {
            return;
        }

        (slingshot.attachments[0], slingshot.attachments[1]) = (slingshot.attachments[1], slingshot.attachments[0]);
        Game1.playSound("button1");
        if (CombatModule.ShouldEnable)
        {
            Slingshot_Stats.Invalidate(slingshot);
        }
    }
}
