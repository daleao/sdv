using JetBrains.Annotations;
using StardewModdingAPI.Events;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;

namespace DaLion.Stardew.Professions.Framework.Events.Player;

[UsedImplicitly]
internal class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object sender, LevelChangedEventArgs e)
    {
        ModEntry.EventManager.Enable(typeof(RestoreForgottenRecipesDayStartedEvent));
    }
}