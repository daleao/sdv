namespace DaLion.Ligo.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Commands;
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
    public override string Documentation => "Remove all enchantments from the selected weapon or slingshot.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var tool = Game1.player.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot))
        {
            Log.W("You must select a weapon or slingshot first.");
            return;
        }

        foreach (var enchantment in tool.enchantments)
        {
            tool.RemoveEnchantment(enchantment);
        }

        Log.I($"Removed all enchantments from {tool.DisplayName}.");
    }
}
