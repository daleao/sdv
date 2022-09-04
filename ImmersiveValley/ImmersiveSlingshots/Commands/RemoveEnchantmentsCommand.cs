namespace DaLion.Stardew.Slingshots.Commands;

#region using directives

using Common;
using Common.Commands;
using LinqFasterer;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RemoveEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RemoveEnchantmentsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "remove_enchants", "remove", "disenchant" };

    /// <inheritdoc />
    public override string Documentation => "Remove the specified enchantments from the selected slingshot." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not Slingshot slingshot)
        {
            Log.W("You must select a slingshot first.");
            return;
        }

        while (args.Length > 0)
        {
            var enchantment = slingshot.enchantments.FirstOrDefaultF(e =>
                e.GetType().Name.ToLowerInvariant().Contains(args[0].ToLowerInvariant()));

            if (enchantment is null)
            {
                Log.W($"The {slingshot.DisplayName} does not have a {args[0]} enchantment.");
                args = args.SkipF(1).ToArrayF();
                continue;
            }

            slingshot.RemoveEnchantment(enchantment);
            Log.I($"Removed {enchantment.GetDisplayName()} enchantment from {slingshot.DisplayName}.");

            args = args.SkipF(1).ToArrayF();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.FirstF()} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a slingshot enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.FirstF()} gatling";
        return result;
    }
}