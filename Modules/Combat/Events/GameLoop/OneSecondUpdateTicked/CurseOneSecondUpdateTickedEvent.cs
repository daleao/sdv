namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CurseOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CurseOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CurseOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (!Game1.game1.ShouldTimePass() || !e.IsMultipleOf(300))
        {
            return;
        }

        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon weapon || !weapon.hasEnchantmentOfType<CursedEnchantment>())
        {
            this.Disable();
            return;
        }

        var dot = weapon.Read<int>(DataKeys.CursePoints) / 10f * CombatModule.Config.RuinBladeDotMultiplier;
        player.health = (int)Math.Max(player.health - dot, 1);
    }
}
