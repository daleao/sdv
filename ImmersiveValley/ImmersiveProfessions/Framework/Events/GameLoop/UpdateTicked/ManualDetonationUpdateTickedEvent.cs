namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ManualDetonationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ManualDetonationUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ManualDetonationUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.ModKey.IsDown())
        {
            return;
        }

        foreach (var sprite in Game1.currentLocation.TemporarySprites.Where(sprite =>
                     sprite.bombRadius > 0 && sprite.totalNumberOfLoops == int.MaxValue))
        {
            sprite.currentNumberOfLoops = sprite.totalNumberOfLoops - 1;
        }

        this.Disable();
    }
}
