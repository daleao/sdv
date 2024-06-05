namespace DaLion.Enchantments.Commands;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FullyEnergizedCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class FullyEnergizedCommand(CommandHandler handler) : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = { "energize" };

    /// <inheritdoc />
    public override string Documentation => "Fully energizes the player, if they currently hold an Energize-enchanted weapon.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        switch (Game1.player.CurrentTool)
        {
            case MeleeWeapon weapon when weapon.GetEnchantmentOfType<EnergizedMeleeEnchantment>() is { } energized:
                energized.Energy = 100;
                break;

            case Slingshot slingshot when slingshot.GetEnchantmentOfType<EnergizedSlingshotEnchantment>() is { } energized:
                energized.Energy = 100;
                break;

            default:
                Log.W("An Energized weapon is not equipped.");
                return false;
        }

        return true;
    }
}
