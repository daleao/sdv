namespace DaLion.Enchantments.Commands;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="WabbajackCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class WabbajackCommand(CommandHandler handler) : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["wabbajack", "wabba", "wab", "wj"];

    /// <inheritdoc />
    public override string Documentation => "Transforms the nearest monster.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var player = Game1.player;
        var nearest = player.GetClosestCharacter<Monster>();
        if (nearest is null)
        {
            Log.W("There are no monsters nearby.");
            return true;
        }

        var dummy = 10;
        WabbajackEnchantment.DoWabbajack(nearest, player.currentLocation, player, ref dummy, 1d);
        return true;
    }
}
