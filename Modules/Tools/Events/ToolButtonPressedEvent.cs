namespace DaLion.Overhaul.Modules.Tools.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Shared.Enums;
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
    public override bool IsEnabled => ToolsModule.Config.EnableAutoSelection || ToolsModule.Config.FaceMouseCursor;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null || Game1.isFestival() || !e.Button.IsUseToolButton())
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not { } tool || player.UsingTool || player.isRidingHorse() || !player.CanMove)
        {
            return;
        }

        if (ToolsModule.Config.FaceMouseCursor && !Game1.options.gamepadControls && (tool is not MeleeWeapon weapon || weapon.isScythe()))
        {
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        var grabTile = e.Cursor.GrabTile;
        if (ToolsModule.Config.EnableAutoSelection &&
            ToolSelector.TryFor(grabTile, player, player.currentLocation, out var selectable1))
        {
            player.CurrentToolIndex = selectable1.Value.Index;
        }
    }
}
