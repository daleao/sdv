namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common;
using Common.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RascalButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RascalButtonPressedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if (Game1.activeClickableMenu is not null || player.CurrentTool is not Slingshot || !player.CanMove ||
            player.canOnlyWalk || player.isRidingHorse() || player.onBridge.Value || player.usingSlingshot) return;

        if (e.Button.IsActionButton())
        {
            ModEntry.State.UsingSecondaryAmmo = true;
            Game1.player.BeginUsingTool();
            Log.D("Charging secondary ammo!");
        }
        else if (e.Button.IsUseToolButton())
        {
            ModEntry.State.UsingPrimaryAmmo = true;
            Log.D("Charging primary ammo!");
        }
    }
}