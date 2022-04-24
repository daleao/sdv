namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicked"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class UpdateTickedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        Log.D("[Arsenal] Hooked UpdateTicked event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
        Log.D("[Arsenal] Unhooked UpdateTicked event.");
    }

    /// <summary>Raised after the player pressed a keyboard, mouse, or controller button.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (!ModEntry.Config.WeaponsCostStamina || Game1.player.CurrentTool is not Slingshot ||
            !Game1.player.usingSlingshot) return;

        if (e.IsMultipleOf(30)) Game1.player.Stamina -= (1 - Game1.player.CombatLevel * 0.05f);
    }
}