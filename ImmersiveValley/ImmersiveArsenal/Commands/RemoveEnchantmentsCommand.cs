namespace DaLion.Stardew.Arsenal.Commands;

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
    public override string Documentation => "Remove the specified enchantments from the selected weapon." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        while (args.Length > 0)
        {
            var enchantment = weapon.enchantments.FirstOrDefaultF(e =>
                e.GetType().Name.ToLowerInvariant().Contains(args[0].ToLowerInvariant()));

            if (enchantment is null)
            {
                Log.W($"The {weapon.DisplayName} does not have a {args[0]} enchantment.");
                args = args.SkipF(1).ToArrayF();
                continue;
            }

            weapon.RemoveEnchantment(enchantment);
            Log.I($"Removed {enchantment.GetDisplayName()} enchantment from {weapon.DisplayName}.");

            args = args.SkipF(1).ToArrayF();
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.FirstF()} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a tool enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.FirstF()} vampiric";
        return result;
    }
}