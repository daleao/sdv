namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using GameLoop;

#endregion using directives

[UsedImplicitly]
internal class PrestigeLevelChangedEvent : LevelChangedEvent
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object sender, LevelChangedEventArgs e)
    {
        EventManager.Enable(typeof(RestoreForgottenRecipesDayStartedEvent));
    }
}