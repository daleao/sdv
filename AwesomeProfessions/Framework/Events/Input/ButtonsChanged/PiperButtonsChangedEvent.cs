namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class PiperButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.ModKey.JustPressed()) return;

        ++ModEntry.State.Value.KeyPressAccumulator;
        if (ModEntry.State.Value.KeyPressAccumulator <= 1) return;

        ModEntry.State.Value.PipeMode = 1 - ModEntry.State.Value.PipeMode;
        Game1.playSound("objectiveComplete");
        Log.D($"Toggled {ModEntry.State.Value.PipeMode} targeting mode.");

        ModEntry.State.Value.KeyPressAccumulator = 0;
    }
}