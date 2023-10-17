namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class MuskUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="MuskUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal MuskUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => GameLocation_Musked.Values.Any();

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        GameLocation_Musked.Values.ForEach(pair => pair.Key.CountdownMusk());
        Monster_Musked.Values.Where(pair => pair.Key.Get_Musked()).ForEach(pair => pair.Key.CountdownMusk());
    }
}
