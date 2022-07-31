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
        : base(manager)
    {
        AlwaysEnabled = true;
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (Context.IsPlayerFree && !player.isRidingHorse() && e.Button.IsUseToolButton() && !player.UsingTool &&
            player.CanMove && player.CurrentTool is Axe or Hoe or Pickaxe or WateringCan &&
            !Game1.options.gamepadControls)
            player.FaceTowardsTile(Game1.currentCursorTile);
    }
}