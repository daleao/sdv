namespace DaLion.Stardew.Tools.Framework.Events;

#region using directives

using Common.Events;
using Common.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ToolButtonPressedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return;

        var player = Game1.player;
        if (e.Button.IsUseToolButton() && !Game1.options.gamepadControls &&
            player.CurrentTool is Axe or Hoe or Pickaxe or WateringCan && !player.UsingTool && !player.isRidingHorse())
            player.FaceTowardsTile(Game1.currentCursorTile);
    }
}