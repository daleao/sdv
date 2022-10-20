namespace DaLion.Stardew.Slingshots.Commands;

#region using directives

using DaLion.Common.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ClearEnchantmentsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearEnchantmentsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_enchants", "clear", "reset" };

    /// <inheritdoc />
    public override string Documentation => "Remove all enchantments from the selected slingshot.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not Slingshot slingshot)
        {
            Log.W("You must select a slingshot first.");
            return;
        }

        foreach (var enchantment in slingshot.enchantments)
        {
            slingshot.RemoveEnchantment(enchantment);
        }

        Log.I($"Removed all enchantments from {slingshot.DisplayName}.");
    }
}
