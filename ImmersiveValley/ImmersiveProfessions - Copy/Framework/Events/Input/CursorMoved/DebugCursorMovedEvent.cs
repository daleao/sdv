namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common.Attributes;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly, DebugOnly]
internal sealed class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugCursorMovedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object? sender, CursorMovedEventArgs e)
    {
        ModEntry.DebugCursorPosition = e.NewPosition;
    }
}