namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Attributes;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly, DebugOnly]
internal sealed class DebugUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        // show FPS counter
        ModEntry.FpsCounter?.Update(Game1.currentGameTime);
    }
}