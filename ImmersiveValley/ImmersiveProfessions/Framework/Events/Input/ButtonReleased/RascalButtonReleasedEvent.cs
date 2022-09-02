using StardewValley.Tools;

namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common;
using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class RascalButtonReleasedEvent : ButtonReleasedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RascalButtonReleasedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonReleasedImpl(object? sender, ButtonReleasedEventArgs e)
    {
        var player = Game1.player;
        if (player.CurrentTool is not Slingshot slingshot || !player.usingSlingshot) return;

        if (e.Button.IsActionButton())
        {
            slingshot.onRelease(player.currentLocation, Game1.getMouseX(), Game1.getMouseY(), player);
            Game1.delayedActions.Add(new(50, () =>
            {
                if (!Game1.didPlayerJustLeftClick()) ModEntry.State.UsingSecondaryAmmo = false;
            }));
            Log.D("No longer charging secondary ammo!");
        }
        else if (e.Button.IsUseToolButton())
        {
            Game1.delayedActions.Add(new (50, () =>
            {
                if (!Game1.didPlayerJustRightClick()) ModEntry.State.UsingPrimaryAmmo = false;
            }));
            Log.D("No longer charging primary ammo!");
        }
    }
}