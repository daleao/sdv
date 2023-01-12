namespace DaLion.Overhaul.Modules.Rings.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using Netcode;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class AddGemstonesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AddGemstonesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddGemstonesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_gemstones", "add" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified gemstones to the selected infinity band.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!RingsModule.Config.TheOneInfinityBand || !Globals.InfinityBandIndex.HasValue)
        {
            Log.W("The One Infinity Band feature is not enabled.");
            return;
        }

        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No gemstones were specified.");
            return;
        }

        if (Game1.player.CurrentItem is not Ring ring || ring.ParentSheetIndex != Globals.InfinityBandIndex.Value)
        {
            Log.W("You must select an Infinity Band first.");
            return;
        }

        var band = ring is CombinedRing combined ? combined : new CombinedRing(880);
        while (args.Length > 0 && band.combinedRings.Count < 4)
        {
            var ringIndex = args[0].ToLower() switch
            {
                // forges
                "ruby" => Constants.RubyRingIndex,
                "aquamarine" => Constants.AquamarineRingIndex,
                "jade" => Constants.JadeRingIndex,
                "emerald" => Constants.EmeraldRingIndex,
                "amethyst" => Constants.AmethystRingIndex,
                "topaz" => Constants.TopazRingIndex,
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

        if (args.Length > 0)
        {
            Log.W($"The selected Infinity Band is full. {args.ToListString()} could not be added.");
        }

        band.ParentSheetIndex = Globals.InfinityBandIndex.Value;
        ModHelper.Reflection.GetField<NetInt>(band, nameof(Ring.indexInTileSheet)).GetValue()
            .Set(Globals.InfinityBandIndex.Value);
        band.UpdateDescription();
        Game1.player.Items[Game1.player.CurrentToolIndex] = band;
    }
}
