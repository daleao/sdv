namespace DaLion.Overhaul.Modules.Ponds.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Ponds.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class UnlockPopulationGatesCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="UnlockPopulationGatesCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal UnlockPopulationGatesCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "unlock_gates", "unlock", "gates" };

    /// <inheritdoc />
    public override string Documentation =>
        "Unlock the specified population gate for the nearest pond, or the all gates if none are specified, and set the max occupants to the maximum value.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var nearest = Game1.player.GetClosestBuilding<FishPond>(predicate: b =>
            b.IsOwnedBy(Game1.player) && !b.isUnderConstruction());
        if (nearest is null)
        {
            Log.W("There are no owned ponds nearby.");
            return;
        }

        if (nearest.fishType.Value < 0)
        {
            Log.W("The nearest pond does not have a registered fish type. Try dropping a fish in it first.");
            return;
        }

        if (nearest.HasUnlockedFinalPopulationGate())
        {
            Log.W("The nearest pond has no population gates left to unlock.");
            return;
        }

        var data = nearest.GetFishPondData();
        int gate;
        switch (args.Length)
        {
            case > 0 when int.TryParse(args[0], out var gateIndex) && gateIndex > 0:
                gate = gateIndex <= data.PopulationGates.Keys.Count
                    ? data.PopulationGates.Keys.ElementAt(gateIndex - 1)
                    : data.PopulationGates.Keys.Max();
                break;
            case > 0:
                Log.W($"{args[0]} is not a valid population gate.");
                return;
            default:
                gate = data.PopulationGates.Keys.Max();
                break;
        }

        nearest.lastUnlockedPopulationGate.Value = gate;
        nearest.UpdateMaximumOccupancy();
        Log.I(args.Length > 0
            ? $"Unlocked {args[0]} population gates for nearby {nearest.GetFishObject().Name} pond."
            : $"Unlocked all population gates for nearby {nearest.GetFishObject().Name} pond.");
    }
}
