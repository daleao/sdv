namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using Common;
using Common.Events;
using Events.Toxicity;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of alchemy events.</summary>
internal class AlchemyEventManager : EventManager
{
    /// <summary>Construct an instance.</summary>
    public AlchemyEventManager(IModEvents modEvents)
    : base(modEvents)
    {
        Log.D("[EventManager]: Enabling Alchemy mod events...");

        #region hookers

        foreach (var @event in ManagedEvents.OfType<ToxicityChangedEvent>())
            ToxicityManager.Changed += @event.OnChanged;

        foreach (var @event in ManagedEvents.OfType<ToxicityClearedEvent>())
            ToxicityManager.Cleared += @event.OnCleared;

        foreach (var @event in ManagedEvents.OfType<ToxicityFilledEvent>())
            ToxicityManager.Filled += @event.OnFilled;

        foreach (var @event in ManagedEvents.OfType<PlayerOverdosedEvent>())
            ToxicityManager.Overdosed += @event.OnOverdosed;

        Log.D($"[EventManager]: Initialization of Alchemy mod events completed.");

        #endregion hookers
    }

    /// <inheritdoc />
    internal override void EnableForLocalPlayer()
    {
        Log.D($"[EventManager]: Enabling profession events for {Game1.player.Name}...");



        Log.D($"[EventManager]: Done enabling event for {Game1.player.Name}.");
    }
}