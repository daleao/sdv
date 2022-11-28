namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.Display;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionSaveLoadedEvent(EventManager manager)
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

        if (player.HasProfession(Profession.Prospector))
        {
            ModEntry.Events.Enable<ProspectorHuntRenderedHudEvent>();
        }

        if (player.HasProfession(Profession.Scavenger))
        {
            ModEntry.Events.Enable<ScavengerHuntRenderedHudEvent>();
        }

        this.Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}
