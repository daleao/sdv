namespace DaLion.Stardew.Alchemy.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class DebugCursorMovedEvent : CursorMovedEvent
{
    /// <inheritdoc />
    protected override void OnCursorMovedImpl(object sender, CursorMovedEventArgs e)
    {
        ModEntry.DebugCursorPosition = e.NewPosition;
    }
}