namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of arsenal events.</summary>
internal class ArsenalEventManager : EventManager
{
    /// <summary>Construct an instance.</summary>
    public ArsenalEventManager(IModEvents modEvents)
        : base(modEvents) { }

    /// <inheritdoc />
    internal override void EnableForLocalPlayer()
    {
        Enable(typeof(ArsenalAssetRequestedEvent), typeof(ArsenalButtonPressedEvent), typeof(ArsenalGameLaunchedEvent),
            typeof(ArsenalSavedEvent), typeof(ArsenalSaveLoadedEvent), typeof(SavingEvent));
    }
}