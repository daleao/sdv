namespace DaLion.Overhaul.Modules.Combat.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.Integrations;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Commands;
using DaLion.Shared.Constants;
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
    public override string[] Triggers { get; } = { "add_gemstones", "add_gems", "ensocket" };

    /// <inheritdoc />
    public override string Documentation => "Add the specified gemstones to the selected infinity band.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (!CombatModule.Config.EnableInfinityBand || !JsonAssetsIntegration.InfinityBandIndex.HasValue)
        {
            Log.W("The One Infinity Band feature is not enabled.");
            return;
        }

        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No gemstones were specified.");
            return;
        }

        if (Game1.player.CurrentItem is not Ring ring || ring.ParentSheetIndex != JsonAssetsIntegration.InfinityBandIndex.Value)
        {
            Log.W("You must select an Infinity Band first.");
            return;
        }

        var band = ring is CombinedRing combined ? combined : new CombinedRing(ObjectIds.CombinedRing);
        while (args.Length > 0 && band.combinedRings.Count < 4)
        {
            var ringIndex = args[0].ToLower() switch
            {
                // forges
                "ruby" => ObjectIds.RubyRing,
                "aquamarine" => ObjectIds.AquamarineRing,
                "jade" => ObjectIds.JadeRing,
                "emerald" => ObjectIds.EmeraldRing,
                "amethyst" => ObjectIds.AmethystRing,
                "topaz" => ObjectIds.TopazRing,
                "garnet" => JsonAssetsIntegration.GarnetRingIndex!.Value,
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

        band.ParentSheetIndex = JsonAssetsIntegration.InfinityBandIndex.Value;
        ModHelper.Reflection.GetField<NetInt>(band, nameof(Ring.indexInTileSheet)).GetValue()
            .Set(JsonAssetsIntegration.InfinityBandIndex.Value);
        band.UpdateDescription();
        CombinedRing_Chord.Values.Remove(band);
        Game1.player.Items[Game1.player.CurrentToolIndex] = band;
    }
}
