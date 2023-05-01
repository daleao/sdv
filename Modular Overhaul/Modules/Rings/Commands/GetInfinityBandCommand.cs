namespace DaLion.Overhaul.Modules.Rings.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using Netcode;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class GetInfinityBandCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="GetInfinityBandCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal GetInfinityBandCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "get_band", "get", "infinity", "inf", "band" };

    /// <inheritdoc />
    public override string Documentation => "Spawns a new Infinity Band with the specified gemstones and adds it to the player.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!RingsModule.Config.TheOneInfinityBand || !Globals.InfinityBandIndex.HasValue)
        {
            Log.W("The One Infinity Band feature is not enabled.");
            return;
        }

        var band = new CombinedRing(880);
        while (args.Length > 0 && band.combinedRings.Count < 4)
        {
            var ringIndex = args[0].ToLower() switch
            {
                // forges
                "ruby" => ItemIDs.RubyRing,
                "aquamarine" => ItemIDs.AquamarineRing,
                "jade" => ItemIDs.JadeRing,
                "emerald" => ItemIDs.EmeraldRing,
                "amethyst" => ItemIDs.AmethystRing,
                "topaz" => ItemIDs.TopazRing,
                "garnet" => Globals.GarnetRingIndex!.Value,
                _ => -1,
            };

            if (ringIndex < 0)
            {
                Log.W($"Ignoring unknown gemstone {args[0]}.");
                args = args.Skip(1).ToArray();
                continue;
            }

            band.combinedRings.Add(new Ring(ringIndex));
            Log.I($"{args[0].FirstCharToUpper()} was added to the Infinity Band.");

            args = args.Skip(1).ToArray();
        }

        if (!Game1.player.addItemToInventoryBool(band))
        {
            Log.W("Your bag is full.");
            return;
        }

        band.ParentSheetIndex = Globals.InfinityBandIndex.Value;
        ModHelper.Reflection.GetField<NetInt>(band, nameof(Ring.indexInTileSheet)).GetValue()
            .Set(Globals.InfinityBandIndex.Value);
        band.UpdateDescription();
        CombinedRing_Chord.Values.Remove(band);
    }
}
