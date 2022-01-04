using JetBrains.Annotations;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.DayStarted;

namespace TheLion.Stardew.Professions.Framework.Events.Player.LevelChanged;

[UsedImplicitly]
internal class StaticLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    public override void OnLevelChanged(object sender, LevelChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        if (e.NewLevel == 0)
        {
            // clean up rogue events and data on skill reset
            ModEntry.Subscriber.CleanUpRogueEvents();
            ModEntry.Data.Value.CleanUpRogueData();
        }
        else
        {
            ModEntry.Subscriber.Subscribe(new RestoreForgottenRecipesDayStartedEvent());
        }
    }
}