namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using Common;
using Common.Commands;
using Framework.Enchantments;
using LinqFasterer;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_enchants", "add", "enchant" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantments to the selected weapon." + GetUsage();

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
            BaseEnchantment? enchantment = args[0].ToLower() switch
            {
                // forges
                "ruby" => new RubyEnchantment(),
                "aquamarine" => new AquamarineEnchantment(),
                "jade" => new JadeEnchantment(),
                "emerald" => new EmeraldEnchantment(),
                "amethyst" => new AmethystEnchantment(),
                "topaz" => new TopazEnchantment(),
                "diamond" => new DiamondEnchantment(),

                // weapon enchants
                "artful" => new ArchaeologistEnchantment(),
                "bugkiller" => new BugKillerEnchantment(),
                "crusader" => new CrusaderEnchantment(),
                "vampiric" => new VampiricEnchantment(),
                "haymaker" => new HaymakerEnchantment(),
                "magic" or "sunburst" => new MagicEnchantment(),
                "cleaving" => new CleavingEnchantment(),
                "energized" => new EnergizedEnchantment(),
                "tribute" or "gold" => new TributeEnchantment(),

                _ => null
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                args = args.SkipF(1).ToArrayF();
                continue;
            }

            if (!enchantment.CanApplyTo(weapon))
            {
                Log.W($"Cannot apply {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");
                args = args.SkipF(1).ToArrayF();
                continue;
            }

            weapon.enchantments.Add(enchantment);
            Log.I($"Applied {enchantment.GetDisplayName()} enchantment to {weapon.DisplayName}.");

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