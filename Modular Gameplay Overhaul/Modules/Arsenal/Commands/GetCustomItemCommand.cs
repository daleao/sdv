namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Networking;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class GetCustomItemCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="GetCustomItemCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal GetCustomItemCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "get" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified DGA custom item to the local player's inventory.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify one of \"Hero Soul\", \"Dwarvish Scrap\" or \"Dwarvish Blueprint\".");
        }

        Item? item = null;
        switch (args[0].ToLowerInvariant())
        {
            case "herosoul":
            case "soul":
                if (!Globals.HeroSoulIndex.HasValue)
                {
                    Log.W("Hero Soul item was not initialized correctly.");
                    return;
                }

                item = new SObject(Globals.HeroSoulIndex.Value, 1);
                break;
            case "dwarvenscrap":
            case "scrap":
                if (!Globals.DwarvenScrapIndex.HasValue)
                {
                    Log.W("Dwarvish Scrap item was not initialized correctly.");
                    return;
                }

                item = new SObject(Globals.DwarvenScrapIndex.Value, 10);
                break;
            case "fairywood":
            case "elderwood":
            case "wood":
                if (!Globals.ElderwoodIndex.HasValue)
                {
                    Log.W("Elderwood item was not initialized correctly.");
                    return;
                }

                item = new SObject(Globals.ElderwoodIndex.Value, 10);
                break;
            case "dwarvish blueprint":
            case "blueprint":
            case "bp":
                if (!Globals.DwarvishBlueprintIndex.HasValue)
                {
                    Log.W("Dwarvish Blueprint item was not initialized correctly.");
                    return;
                }

                var player = Game1.player;
                var allBlueprints = new List<int>
                {
                    Constants.ElfBladeIndex,
                    Constants.ForestSwordIndex,
                    Constants.DwarfSwordIndex,
                    Constants.DwarfHammerIndex,
                    Constants.DwarfDaggerIndex,
                    Constants.DragontoothCutlassIndex,
                    Constants.DragontoothClubIndex,
                    Constants.DragontoothShivIndex,
                };

                if (args.Length > 1)
                {
                    switch (args[1].ToLowerInvariant())
                    {
                        case "all":
                            player.Write(DataFields.BlueprintsFound, string.Join(',', allBlueprints));
                            Log.I($"Added all Dwarvish Blueprints to {player.Name}.");
                            break;
                        case "none":
                            player.Write(DataFields.BlueprintsFound, null);
                            Log.I($"Removed all Dwarvish Blueprints from {player.Name}.");
                            break;
                    }

                    return;
                }

                var notFound = allBlueprints.Except(player.Read(DataFields.BlueprintsFound).ParseList<int>()).ToArray();
                var chosen = Game1.random.Next(notFound.Length);
                player.Append(DataFields.BlueprintsFound, chosen.ToString());
                ModHelper.GameContent.InvalidateCache("Data/Events/Blacksmith");

                player.holdUpItemThenMessage(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
                if (Context.IsMultiplayer)
                {
                    Broadcaster.SendPublicChat(I18n.Get("blueprint.found.global", new { who = player.Name }));
                }

                player.addItemToInventoryBool(new SObject(Globals.DwarvishBlueprintIndex.Value, 1));
                return;
            case "blade":
            case "ruin":
            case "ruined":
            case "curse":
            case "cursed":
                item = new MeleeWeapon(Constants.DarkSwordIndex);
                break;
            default:
                Log.W($"Invalid item {args[0]} will be ignored.");
                break;
        }

        if (item is not null)
        {
            Utility.CollectOrDrop(item);
        }
    }
}
