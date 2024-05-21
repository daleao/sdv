namespace DaLion.Professions.Framework.Events.Player.LevelChanged;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RestoreForgottenRecipesLevelChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class RestoreForgottenRecipesLevelChangedEvent(EventManager? manager = null)
    : LevelChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        this.Manager.Enable<RestoreForgottenRecipesDayStartedEvent>();
    }
}
