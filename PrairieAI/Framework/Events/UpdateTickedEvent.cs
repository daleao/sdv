using StardewValley.Menus;
using StardewValley.Minigames;

namespace DaLion.Stardew.Prairie.Training.Framework.Events;

#region using directives

using System.Reflection;
using JetBrains.Annotations;
using SharpNeat.Core;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions.Reflection;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.UpdateTicked"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class UpdateTickedEvent : IEvent
{
    private static readonly MethodInfo _OverrideButton = Game1.input.GetType().RequireMethod("OverrideButton");

    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        Log.D("[Prairie] Hooked UpdateTicked event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
        Log.D("[Prairie] Unhooked UpdateTicked event.");
    }

    /// <summary>Raised after the game state is updated.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        // wait until game window opens
        if (Game1.ticks <= 1) return;
        
        // start minigame
        if (Game1.activeClickableMenu is TitleMenu)
            Game1.currentMinigame = new AbigailGame();
        else if (ModEntry.IsPlayingAbigailGame && ModEntry.Network?.RunState is RunState.Running)
            foreach (var (button, shouldPress) in ModEntry.Actions)
                if (shouldPress)
                    _OverrideButton.Invoke(Game1.input, new object[] {button, true});
    }
}