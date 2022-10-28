namespace DaLion.Redux.Professions.Events.Player;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabled]
internal sealed class StaticLevelChangedEvent : LevelChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticLevelChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StaticLevelChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        if (e.Skill != SkillType.Combat || e.NewLevel % 5 != 0)
        {
            return;
        }

        if (e.NewLevel > e.OldLevel)
        {
            Game1.player.maxHealth += 5;
            Game1.player.health = Game1.player.maxHealth;
        }
        else if (e.NewLevel < e.OldLevel)
        {
            Game1.player.maxHealth -= 5;
            Game1.player.health = Math.Max(Game1.player.health, Game1.player.maxHealth);
        }
    }
}
