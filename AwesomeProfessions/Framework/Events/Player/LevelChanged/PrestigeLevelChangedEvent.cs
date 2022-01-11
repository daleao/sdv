using JetBrains.Annotations;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

namespace TheLion.Stardew.Professions.Framework.Events.Player.LevelChanged;

[UsedImplicitly]
internal class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object sender, LevelChangedEventArgs e)
    {
        ModEntry.EventManager.Enable(typeof(RestoreForgottenRecipesDayStartedEvent));
    }
}