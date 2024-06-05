namespace DaLion.Enchantments.Commands;

#region using directives

using DaLion.Enchantments.Framework.Animations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="OverHealCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class OverHealCommand(CommandHandler handler) : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = { "overheal" };

    /// <inheritdoc />
    public override string Documentation => "Heals the player, allowing health to go above the maximum value.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var player = Game1.player;
        player.health = (int)(player.maxHealth * 1.2f);
        ShieldAnimation.Instance = new ShieldAnimation(player);
        return true;
    }
}
