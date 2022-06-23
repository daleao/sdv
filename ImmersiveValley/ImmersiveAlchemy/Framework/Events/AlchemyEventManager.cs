namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Events;
using Events.Toxicity;

#endregion using directives

/// <summary>Manages dynamic hooking and unhooking of alchemy events.</summary>
internal class AlchemyEventManager : EventManager
{
    /// <summary>Construct an instance.</summary>
    public AlchemyEventManager(IModEvents modEvents)
    : base(modEvents)
    {
        Log.D("[EventManager]: Hooking Alchemy mod events...");

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

#if DEBUG
        HookStartingWith("Debug");
#endif
    }

    /// <inheritdoc />
    internal override void HookForLocalPlayer()
    {
        Log.D($"[EventManager]: Hooking profession events for {Game1.player.Name}...");



        Log.D($"[EventManager]: Done hooking event for {Game1.player.Name}.");
    }
}