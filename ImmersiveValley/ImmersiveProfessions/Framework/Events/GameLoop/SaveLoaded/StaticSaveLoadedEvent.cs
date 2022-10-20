namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticSaveLoadedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Disable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;

        ModEntry.Events.EnableForLocalPlayer();

        player.RevalidateLevels();

        player.RevalidateUltimate();

        Game1.game1.RevalidateFishPondPopulations();

        this.Manager.Enable<PrestigeAchievementOneSecondUpdateTickedEvent>();
    }
}
