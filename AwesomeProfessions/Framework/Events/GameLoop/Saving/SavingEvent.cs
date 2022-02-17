namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class SavingEvent : BaseEvent
{
    /// <summary>Raised before the game writes data to save file.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnSaving(object sender, SavingEventArgs e)
    {
        if (enabled.Value) OnSavingImpl(sender, e);
    }

    /// <inheritdoc cref="OnSaving" />
    protected abstract void OnSavingImpl(object sender, SavingEventArgs e);
}