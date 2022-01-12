using JetBrains.Annotations;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.GameLoop;

namespace TheLion.Stardew.Professions.Framework.Events.Player;

[UsedImplicitly]
internal class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object sender, LevelChangedEventArgs e)
    {
        ModEntry.EventManager.Enable(typeof(RestoreForgottenRecipesDayStartedEvent));
    }
}