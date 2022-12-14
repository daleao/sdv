namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ToolButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ToolsModule.Config.FaceMouseCursor;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || !e.Button.IsUseToolButton() ||
            Game1.options.gamepadControls)
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is Axe or Hoe or Pickaxe or WateringCan && !player.UsingTool &&
            !player.isRidingHorse() && player.CanMove)
        {
            player.FaceTowardsTile(Game1.currentCursorTile);
        }
    }
}
