namespace DaLion.Overhaul.Modules.Tools.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AddEnchantmentCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_enchantment", "add_ench", "enchant", "ench" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantments to the currently selected tool.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is null or not (Axe or Hoe or Pickaxe or WateringCan or FishingRod))
        {
            Log.W("You must select a tool first.");
            return;
        }

        BaseEnchantment? enchantment = args[0].ToLower() switch
        {
            "auto-hook" or "autohook" or "hook" => new AutoHookEnchantment(),
            "arch" or "archaeologist" => new ArchaeologistEnchantment(),
            "bottomless" => new BottomlessEnchantment(),
            "efficient" => new EfficientToolEnchantment(),
            "generous" => new GenerousEnchantment(),
            "master" => new MasterEnchantment(),
            "powerful" => new PowerfulEnchantment(),
            "preserving" when tool is FishingRod => new StardewValley.PreservingEnchantment(),
            "reaching" => new ReachingToolEnchantment(),
            "shaving" => new ShavingEnchantment(),
            "swift" => new SwiftToolEnchantment(),
            _ => null,
        };

        if (enchantment is null)
        {
            Log.W($"Ignoring unknown enchantment {args[0]}.");
            args = args.Skip(1).ToArray();
            return;
        }

        if (!enchantment.CanApplyTo(tool))
        {
            Log.W($"Cannot apply {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");
            args = args.Skip(1).ToArray();
            return;
        }

        tool.AddEnchantment(enchantment);
        Log.I($"Applied {enchantment.GetType().Name} to {tool.DisplayName}.");
    }
}
