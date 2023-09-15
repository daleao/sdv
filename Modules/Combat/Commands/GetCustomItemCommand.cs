namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
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
    public override string[] Triggers { get; } = { "get_item" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified DGA custom item to the local player's inventory.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify one of \"Hero Soul\", \"Dwarvish Scrap\" or \"Dwarvish Blueprint\".");
        }

        Item? item = null;
        switch (args[0].ToLowerInvariant())
        {
            case "herosoul":
            case "soul":
                if (!JsonAssetsIntegration.HeroSoulIndex.HasValue)
                {
                    Log.W("Hero Soul item was not initialized correctly.");
                    return;
                }

                item = new SObject(JsonAssetsIntegration.HeroSoulIndex.Value, 1);
                break;
            case "dwarvenscrap":
            case "scrap":
                if (!JsonAssetsIntegration.DwarvenScrapIndex.HasValue)
                {
                    Log.W("Dwarvish Scrap item was not initialized correctly.");
                    return;
                }

                item = new SObject(JsonAssetsIntegration.DwarvenScrapIndex.Value, 10);
                break;
            case "fairywood":
            case "elderwood":
            case "wood":
                if (!JsonAssetsIntegration.ElderwoodIndex.HasValue)
                {
                    Log.W("Elderwood item was not initialized correctly.");
                    return;
                }

                item = new SObject(JsonAssetsIntegration.ElderwoodIndex.Value, 10);
                break;
            case "dwarvish blueprint":
            case "blueprint":
            case "bp":
                if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue)
                {
                    Log.W("Dwarvish Blueprint item was not initialized correctly.");
                    return;
                }

                var player = Game1.player;
                var allBlueprints = new List<int>
                {
                    WeaponIds.ElfBlade,
                    WeaponIds.ForestSword,
                    WeaponIds.DwarfSword,
                    WeaponIds.DwarfHammer,
                    WeaponIds.DwarfDagger,
                    WeaponIds.DragontoothCutlass,
                    WeaponIds.DragontoothClub,
                    WeaponIds.DragontoothShiv,
                };

                if (args.Length > 1)
                {
                    switch (args[1].ToLowerInvariant())
                    {
                        case "all":
                            player.Write(DataKeys.BlueprintsFound, string.Join(',', allBlueprints));
                            Log.I($"Added all Dwarvish Blueprints to {player.Name}.");
                            return;
                        case "none":
                            player.Write(DataKeys.BlueprintsFound, null);
                            Log.I($"Removed all Dwarvish Blueprints from {player.Name}.");
                            return;
                        default:
                            var found = player.Read(DataKeys.BlueprintsFound).ParseList<int>().ToHashSet();
                            for (var i = 0; i < args.Length; i++)
                            {
                                switch (args[i].ToLowerInvariant())
                                {
                                    case "elfblade":
                                    case "elfdagger":
                                    case "elvenblade":
                                    case "elvendagger":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Elven Blade Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.ElfBlade.ToString());
                                        Log.I($"Added the Elven Blade Blueprint to {player.Name}.");
                                        break;
                                    case "elfsword":
                                    case "elvensword":
                                    case "forestsword":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Elven Sword Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.ForestSword.ToString());
                                        Log.I($"Added the Elven Sword Blueprint to {player.Name}.");
                                        break;
                                    case "dwarfsword":
                                    case "dwarvensword":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dwarven Sword Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfSword.ToString());
                                        Log.I($"Added the Dwarven Sword Blueprint to {player.Name}.");
                                        break;
                                    case "dwarfdagger":
                                    case "dwarvendagger":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dwarven Dagger Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfDagger.ToString());
                                        Log.I($"Added the Dwarven Dagger Blueprint to {player.Name}.");
                                        break;
                                    case "dwarfclub":
                                    case "dwarvenclub":
                                    case "dwarfhammer":
                                    case "dwarvenhammer":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dwarven Hammer Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DwarfHammer.ToString());
                                        Log.I($"Added the Dwarven Hammer Blueprint to {player.Name}.");
                                        break;
                                    case "dragonsword":
                                    case "dragoncutlass":
                                    case "dragontoothsword":
                                    case "dragontoothcutlass":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dragontooth Cutlass Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothCutlass.ToString());
                                        Log.I($"Added the Dragontooth Cutlass Blueprint to {player.Name}.");
                                        break;
                                    case "dragondagger":
                                    case "dragonshiv":
                                    case "dragontoothdagger":
                                    case "dragontoothshiv":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dragontooth Shiv Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothShiv.ToString());
                                        Log.I($"Added the Dragontooth Shiv Blueprint to {player.Name}.");
                                        break;
                                    case "dragonclub":
                                    case "dragontoothclub":
                                        if (found.Contains(WeaponIds.ElfBlade))
                                        {
                                            Log.W($"{player.Name} has already found the Dragontooth Club Blueprint.");
                                            break;
                                        }

                                        player.Append(DataKeys.BlueprintsFound, WeaponIds.DragontoothClub.ToString());
                                        Log.I($"Added the Dragontooth Club Blueprints to {player.Name}.");
                                        break;
                                    default:
                                        Log.W($"Ignoring unknown weapon '{args[i]}'.");
                                        break;
                                }
                            }

                            return;
                    }
                }

                var notFound = allBlueprints.Except(player.Read(DataKeys.BlueprintsFound).ParseList<int>()).ToArray();
                var chosen = Game1.random.Next(notFound.Length);
                player.Append(DataKeys.BlueprintsFound, notFound[chosen].ToString());
                ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");

                player.holdUpItemThenMessage(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                if (Context.IsMultiplayer)
                {
                    Broadcaster.SendPublicChat(I18n.Blueprint_Found_Global(player.Name));
                }

                player.addItemToInventoryBool(new SObject(JsonAssetsIntegration.DwarvishBlueprintIndex.Value, 1));
                return;
            case "bladeofruin":
            case "ruinblade":
            case "ruinedblade":
            case "blade":
            case "ruin":
            case "ruined":
            case "bork":
                item = new MeleeWeapon(WeaponIds.DarkSword);
                Game1.player.mailReceived.Add("gotDarkSword");
                Game1.player.mailForTomorrow.Add("viegoCurse");
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
