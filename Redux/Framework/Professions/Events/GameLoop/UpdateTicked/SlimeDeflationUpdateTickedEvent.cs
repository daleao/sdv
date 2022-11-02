namespace DaLion.Redux.Framework.Professions.Events.GameLoop;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Professions.Extensions;
using DaLion.Redux.Framework.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SlimeDeflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SlimeDeflationUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SlimeDeflationUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var undeflated = GreenSlime_Piped.Values.Select(pair => pair.Key).ToArray();
        if (undeflated.Length == 0)
        {
            this.Disable();
            return;
        }

        foreach (var piped in undeflated)
        {
            piped.Deflate();
        }
    }
}
