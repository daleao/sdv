namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigeLevelChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeLevelChangedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        this.Manager.Enable<RestoreForgottenRecipesDayStartedEvent>();
    }
}
