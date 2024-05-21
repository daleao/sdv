namespace DaLion.Professions.Framework.Events.Player.LevelChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CombatLevelChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatLevelChangedEvent(EventManager? manager = null)
    : LevelChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnLevelChangedImpl(object? sender, LevelChangedEventArgs e)
    {
        if (e.Skill != SkillType.Combat || e.NewLevel % 5 != 0)
        {
            return;
        }

        var delta = e.NewLevel - e.OldLevel;
        if (delta > 0)
        {
            Game1.player.maxHealth += 5;
            if (delta > 5)
            {
                Game1.player.maxHealth += 5;
            }

            Game1.player.health = Game1.player.maxHealth;
        }
        else if (delta < 0)
        {
            Game1.player.maxHealth -= 5;
            if (delta < -5)
            {
                Game1.player.maxHealth -= 5;
            }

            Game1.player.health = Math.Max(Game1.player.health, Game1.player.maxHealth);
        }
    }
}
