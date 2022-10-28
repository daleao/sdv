namespace DaLion.Redux.Professions.Events.GameLoop;

#region using directives

using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabled]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StaticSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        Skill.List.ForEach(s => s.Revalidate());
        player.RevalidateUltimate();
        Game1.game1.RevalidateFishPondPopulations();
        ModEntry.Events.EnableForLocalPlayer();
        this.Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}
