namespace DaLion.Stardew.Tools.Commands;

#region using directives

using System.Linq;

using StardewValley;
using StardewValley.Tools;

using Common;
using Common.Commands;

#endregion using directives

internal class GetToolsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "get";

    /// <inheritdoc />
    public string Documentation =>
        "Add missing farming and resource tools to the inventory." +
        "\nTo add only specific tools, use `debug` + `ax`, `pick`, `hoe` or `can` instead.";

    /// <inheritdoc />
    public void Callback(string[] args)
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