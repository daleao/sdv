namespace DaLion.Stardew.Tools.Commands;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Common;
using Common.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class GetToolsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal GetToolsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "get";

    /// <inheritdoc />
    public override string Documentation =>
        "Add missing farming and resource tools to the inventory." +
        "\nTo add only specific tools, use `debug` + `ax`, `pick`, `hoe` or `can` instead.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Game1.player.CurrentTool is not MeleeWeapon weapon)
        {
            Log.W("You must select a weapon first.");
            return;
        }

        if (!Game1.player.Items.OfType<Axe>().Any())
            Game1.player.Items.Add(new Axe().getOne());

        if (!Game1.player.Items.OfType<Pickaxe>().Any())
            Game1.player.Items.Add(new Pickaxe().getOne());

        if (!Game1.player.Items.OfType<Hoe>().Any())
            Game1.player.Items.Add(new Hoe().getOne());

        if (!Game1.player.Items.OfType<WateringCan>().Any())
            Game1.player.Items.Add(new WateringCan().getOne());
    }
}