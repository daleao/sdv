namespace DaLion.Overhaul.Modules.Professions.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeLevelChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigeLevelChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        this.Manager.Enable<RestoreForgottenRecipesDayStartedEvent>();
    }
}
