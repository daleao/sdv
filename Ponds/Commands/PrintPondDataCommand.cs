namespace DaLion.Ponds.Commands;

#region using directives

using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintPondDataCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintPondDataCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintPondDataCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = ["print_data", "print", "data"];

    /// <inheritdoc />
    public override string Documentation => "Print all mod data fields for the nearest pond.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length > 0)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var nearest = Game1.player.GetClosestBuilding<FishPond>(out _, predicate: b =>
            b.IsOwnedBy(Game1.player) && !b.isUnderConstruction());
        if (nearest is null)
        {
            Log.W("There are no owned ponds nearby.");
            return true;
        }

        if (string.IsNullOrEmpty(nearest.fishType.Value))
        {
            var daysEmpty = Data.ReadAs<int>(nearest, DataKeys.DaysEmpty);
            Log.I($"Empty for {daysEmpty} days.");
            return true;
        }

        var fish = nearest.GetFishObject();
        var message = new StringBuilder($"{fish.Name} pond's mod data:");
        var fishQualities = Data.Read(nearest, DataKeys.FishQualities).ParseList<int>();
        message.Append("\n\tFish qualities:")
                .Append($"\n\t\t- Regular: {fishQualities[0]})")
                .Append($"\n\t\t- Silver: {fishQualities[1]}")
                .Append($"\n\t\t- Gold: {fishQualities[2]}")
                .Append($"\n\t\t- Iridium: {fishQualities[3]}");

        if (fish.HasContextTag("fish_legendary"))
        {
            var familyLivingHere = Data.ReadAs<int>(nearest, DataKeys.FamilyLivingHere, modId: "DaLion.Professions");
            message.Append($"\n\n\tExtended family members: {familyLivingHere}");
            if (familyLivingHere > 0)
            {
                var familyQualities = Data.Read(nearest, DataKeys.FamilyQualities).ParseList<int>();
                message.Append("\n\n\tFamily member qualities:")
                    .Append($"\n\t\t- Regular: {familyQualities[0]}")
                    .Append($"\n\t\t- Silver: {familyQualities[1]}")
                    .Append($"\n\t\t- Gold: {familyQualities[2]}")
                    .Append($"\n\t\t- Iridium: {familyQualities[3]}");
            }
        }
        else if (fish.IsAlgae())
        {
            var seaweedLivingHere = Data.ReadAs<int>(nearest, DataKeys.SeaweedLivingHere);
            var greenAlgaeLivingHere = Data.ReadAs<int>(nearest, DataKeys.GreenAlgaeLivingHere);
            var whiteAlgaeLivingHere = Data.ReadAs<int>(nearest, DataKeys.WhiteAlgaeLivingHere);
            message.Append("\n\n\tAlgae species living here:")
                .Append($"\n\t\t- Seaweed: {seaweedLivingHere}")
                .Append($"\n\t\t- Green Algae: {greenAlgaeLivingHere}")
                .Append($"\n\t\t- White Algae: {whiteAlgaeLivingHere}");
        }

        var held = Data.Read(nearest, DataKeys.ItemsHeld).ParseList<string>(';');
        if (held.Count > 0)
        {
            message.Append("\n\n\tAdditional items held:");
            foreach (var item in held.WhereNotNull())
            {
                var (id, stack, quality) = item.ParseTuple<string, int, ObjectQuality>()!.Value;
                var obj = ItemRegistry.Create<SObject>(id, stack, quality: (int)quality);
                message.Append($"\n\t\t- {obj.Name} x{stack} ({quality})");
            }
        }
        else
        {
            message.Append("\n\n\tThe pond holds no items.:");
        }

        var hasOrHasnt = Data.ReadAs<bool>(nearest, DataKeys.CheckedToday) ? "has" : "hasn't";
        message.Append($"\n\n\tThe pond {hasOrHasnt} been checked today.");
        Log.I(message.ToString());
        return true;
    }
}
