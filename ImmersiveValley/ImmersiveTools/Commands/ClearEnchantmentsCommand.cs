namespace DaLion.Stardew.Tools.Commands;

#region using directives

using Common;
using Common.Commands;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ClearEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ClearEnchantmentsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "clear_enchants", "clear", "reset" };

    /// <inheritdoc />
    public override string Documentation => "Remove all enchantments from the selected weapon or slingshot.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not ({ } tool and (Axe or Hoe or Pickaxe or WateringCan or FishingRod)))
        {
            Log.W("You must select a tool first.");
            return;
        }

        foreach (var enchantment in tool.enchantments)
            tool.RemoveEnchantment(enchantment);

        Log.I($"Removed all enchantments from {tool.DisplayName}.");
    }
}