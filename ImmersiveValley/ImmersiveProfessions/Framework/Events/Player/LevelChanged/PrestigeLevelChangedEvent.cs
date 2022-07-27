namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using GameLoop;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal PrestigeLevelChangedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        Manager.Enable<RestoreForgottenRecipesDayStartedEvent>();
    }
}