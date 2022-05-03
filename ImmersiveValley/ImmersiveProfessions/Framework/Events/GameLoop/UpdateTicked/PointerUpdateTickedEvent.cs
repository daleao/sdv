namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class PointerUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.PlayerState.Pointer.Update(e.Ticks);
    }
}