﻿namespace DaLion.Enchantments.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using StardewValley;
using StardewValley.Tools;
using VampiricEnchantment = DaLion.Enchantments.Framework.Enchantments.VampiricEnchantment;
using VanillaVampiricEnchantment = StardewValley.Enchantments.VampiricEnchantment;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="AddEnchantmentsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class AddEnchantmentsCommand(CommandHandler handler) : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["add"];

    /// <inheritdoc />
    public override string Documentation => "Add the specified enchantments to the currently selected tool.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No enchantment was specified.");
            return false;
        }

        var tool = Game1.player.CurrentTool;
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
                "carving" or "carve" => new CarvingEnchantment(),
                "cleaving" or "cleave" => new CleavingEnchantment(),
                "explosive" or "blasting" or "blast" or "volatile" or "revenant" or "reactive" or "biding" or "raging" => new ExplosiveEnchantment(),
                "vampiric" or "vamp" or "bloodthirsty" => new VampiricEnchantment(),
                "mammonite" or "mammon" or "greedy" => new MammoniteEnchantment(),
                "wabbajack" or "wabba" or "wab" or "wb" or "wj" => new WabbajackEnchantment(),
                "stabbing" or "stabby" or "piercing" or "lunging" or "dashing" or "fencing" or "reckless" => new StabbingEnchantment(),
                "sunburst" => new SunburstEnchantment(),

                // slingshot enchants
                "chilling" or "chilly" or "freezing" or "freljord" => new ChillingEnchantment(),
                "quincy" => new QuincyEnchantment(),
                "runaan" or "echo" or "reverb" => new EchoEnchantment(),

                // unisex enchants
                "energized" or "shock" or "shocking" or "statikk" or "thunderlords" => tool is Slingshot ? new EnergizedSlingshotEnchantment() : new EnergizedMeleeEnchantment(),

                // tool enchants
                "autohook" or "auto-hook" => new AutoHookEnchantment(),
                "archaeologist" => new ArchaeologistEnchantment(),
                "bottomless" => new BottomlessEnchantment(),
                "efficient" => new EfficientToolEnchantment(),
                "generous" => new GenerousEnchantment(),
                "fisher" => new FisherEnchantment(),
                "master" => new MasterEnchantment(),
                "powerful" => new PowerfulEnchantment(),
                "preserving" => new PreservingEnchantment(),
                "reaching" => new ReachingToolEnchantment(),
                "shaving" => new ShavingEnchantment(),
                "swift" => new SwiftToolEnchantment(),

                "haymaker" => new HaymakerEnchantment(),
                "sharp" => new SharpEnchantment(),

                // vanilla weapon enchants
                "v_artful" => new ArtfulEnchantment(),
                "v_bugkiller" => new BugKillerEnchantment(),
                "v_crusader" => new CrusaderEnchantment(),
                "v_vampiric" => new VanillaVampiricEnchantment(),
                "v_magic" or "v_sunburst" => new MagicEnchantment(),

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

            // ensure old enchantments are replaced correctly
            tool.enchantments
                .Where(e => !e.IsForge() && !e.IsSecondaryEnchantment())
                .ForEach(tool.RemoveEnchantment);
            tool.AddEnchantment(enchantment);
            Log.I($"Applied {enchantment.GetType().Name} to {tool.DisplayName}.");
            args = args.Skip(1).ToArray();
        }

        return true;
    }

    /// <inheritdoc />
    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <enchantment>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t- <enchantment>: a weapon or slingshot enchantment");
        sb.Append("\n\nExample:");
        sb.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} vampiric");
        return sb.ToString();
    }
}
