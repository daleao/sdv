namespace DaLion.Stardew.Alchemy.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal class DebugUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        // show FPS counter
        ModEntry.FpsCounter?.Update(Game1.currentGameTime);
    }
}