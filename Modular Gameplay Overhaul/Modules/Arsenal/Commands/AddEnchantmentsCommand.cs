namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class AddEnchantmentsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AddEnchantmentsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddEnchantmentsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_enchants", "add", "enchant" };

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified enchantments to the selected weapon or slingshot." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is not (MeleeWeapon or Slingshot))
        {
            Log.W("You must select a weapon or slingshot first.");
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
                "garnet" => new GarnetEnchantment(),

                // weapon enchants
                "haymaker" => new HaymakerEnchantment(),
                "carving" => new CarvingEnchantment(),
                "cleaving" => new CleavingEnchantment(),
                "energized" => new EnergizedEnchantment(),
                "tribute" or "gold" => new TributeEnchantment(),
                "r_artful" => new ReduxArtfulEnchantment(),
                "bloodthirsty" => new BloodthirstyEnchantment(),

                // slingshot enchants
                "engorging" => new EngorgingEnchantment(),
                "gatling" => new GatlingEnchantment(),
                "preserving" => new PreservingEnchantment(),
                "quincy" => new QuincyEnchantment(),
                "spreading" => new SpreadingEnchantment(),

                // vanilla
                "artful" => new ArtfulEnchantment(),
                "bugkiller" => new BugKillerEnchantment(),
                "crusader" => new CrusaderEnchantment(),
                "vampiric" => new VampiricEnchantment(),
                "magic" or "sunburst" => new MagicEnchantment(),

                _ => null,
            };

            if (enchantment is null)
            {
                Log.W($"Ignoring unknown enchantment {args[0]}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                Log.W($"Cannot apply {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            tool.AddEnchantment(enchantment);
            Log.I($"Applied {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }

        tool.Invalidate();
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} <enchantment>";
        result += "\n\nParameters:";
        result += "\n\t- <enchantment>: a weapon or slingshot enchantment";
        result += "\n\nExample:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} vampiric";
        return result;
    }
}
