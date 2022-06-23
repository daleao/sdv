namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object sender, CursorMovedEventArgs e)
    {
        ModEntry.DebugCursorPosition = e.NewPosition;
    }
}