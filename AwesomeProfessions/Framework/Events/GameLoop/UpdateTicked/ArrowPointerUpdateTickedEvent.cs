using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class ArrowPointerUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        HUD.Pointer ??= new();
        if (e.Ticks % 4 == 0) HUD.Pointer.Bob();
    }
}