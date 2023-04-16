namespace DaLion.Overhaul.Modules.Enchantments.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Enchantments.Melee;
using DaLion.Overhaul.Modules.Enchantments.Ranged;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;
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
        "Add the specified enchantments to the currently selected tool." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return;
        }

        var tool = Game1.player.CurrentTool;
        if (tool is null)
        {
            Log.W("You must select a tool first.");
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
                "energized" or "thunderlords" => new EnergizedEnchantment(),
                "explosive" or "blasting" => new ExplosiveEnchantment(),
                "tribute" or "gold" => new TributeEnchantment(),
                "artful" => new NewArtfulEnchantment(),
                "bloodthirsty" or "vampiric" => new BloodthirstyEnchantment(),
                "wabbajack" or "wabba" or "wab" => new WabbajackEnchantment(),

                // slingshot enchants
                "engorging" or "glutton" => new EngorgingEnchantment(),
                "gatling" => new GatlingEnchantment(),
                "preserving" when tool is Slingshot => new Ranged.PreservingEnchantment(),
                "quincy" => new QuincyEnchantment(),
                "spreading" => new SpreadingEnchantment(),

                // removed vanilla weapon enchants
                "v_artful" => new ArtfulEnchantment(),
                "v_bugkiller" => new BugKillerEnchantment(),
                "v_crusader" => new CrusaderEnchantment(),
                "v_vampiric" => new VampiricEnchantment(),
                "magic" or "sunburst" => new MagicEnchantment(),

                // tool enchants
                "auto-hook" or "autohook" => new AutoHookEnchantment(),
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
                continue;
            }

            if (enchantment is GarnetEnchantment && !Globals.GarnetIndex.HasValue)
            {
                Log.W("You must have the Rings module enabled and the Garnet item loaded to use the Garnet enchantment.");
                args = args.Skip(1).ToArray();
                continue;
            }

            if (!enchantment.CanApplyTo(tool))
            {
                Log.W($"Cannot apply {args[0].FirstCharToUpper()} enchantment to {tool.DisplayName}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            // ensure old enchantments are replaced correctly
            tool.enchantments
                .Where(e => !e.IsForge() && !e.IsSecondaryEnchantment())
                .ForEach(e => tool.RemoveEnchantment(e));

            tool.AddEnchantment(enchantment);
            Log.I($"Applied {enchantment.GetType().Name} to {tool.DisplayName}.");

            args = args.Skip(1).ToArray();
        }

        switch (tool)
        {
            case MeleeWeapon weapon when WeaponsModule.ShouldEnable:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.ShouldEnable:
                slingshot.Invalidate();
                break;
        }
    }

    /// <summary>Tell the dummies how to use the console command.</summary>
    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <enchantment>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <enchantment>: a weapon or slingshot enchantment");
        result.Append("\n\nExample:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} vampiric");
        return result.ToString();
    }
}
