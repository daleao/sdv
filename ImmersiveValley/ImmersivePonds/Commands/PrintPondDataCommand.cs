namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Enums;
using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Stardew;
using Extensions;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintPondDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintPondDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_data", "print", "data" };

    /// <inheritdoc />
    public override string Documentation => "Print all mod data fields for the nearest pond.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length > 0)
            Log.W("Additional arguments will be ignored.");

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (ponds.Count < 0)
        {
            Log.W("You don't own any Fish Ponds.");
            return;
        }

        var nearest = Game1.player.GetClosestBuilding(out _, ponds);
        if (nearest is null)
        {
            Log.W("There are no ponds nearby.");
            return;
        }

        if (nearest.fishType.Value < 0)
        {
            var daysEmpty = nearest.Read<int>("DaysEmpty");
            Log.I($"Empty for {daysEmpty} days.");
            return;
        }

        var fish = nearest.GetFishObject();
        var message = $"{fish.Name} pond's mod data:";
        var fishQualities = nearest.Read("FishQualities").ParseList<int>()!;
        message += "\n\tFish qualities:" +
                   $"\n\t\t- Regular: {fishQualities[0]}" +
                   $"\n\t\t- Silver: {fishQualities[1]}" +
                   $"\n\t\t- Gold: {fishQualities[2]}" +
                   $"\n\t\t- Iridium: {fishQualities[3]}";

        if (fish.HasContextTag("fish_legendary"))
        {
            var familyLivingHere = nearest.Read<int>("FamilyLivingHere");
            message += $"\n\n\tExtended family members: {familyLivingHere}";
            if (familyLivingHere > 0)
            {
                var familyQualities = nearest.Read("FamilyQualities").ParseList<int>()!;
                message += "\n\n\tFamily member qualities:" +
                           $"\n\t\t- Regular: {familyQualities[0]}" +
                           $"\n\t\t- Silver: {familyQualities[1]}" +
                           $"\n\t\t- Gold: {familyQualities[2]}" +
                           $"\n\t\t- Iridium: {familyQualities[3]}";
            }
        }
        else if (fish.IsAlgae())
        {

            var seaweedLivingHere = nearest.Read<int>("SeaweedLivingHere");
            var greenAlgaeLivingHere = nearest.Read<int>("GreenAlgaeLivingHere");
            var whiteAlgaeLivingHere = nearest.Read<int>("WhiteAlgaeLivingHere");
            message += "\n\n\tAlgae species living here:" +
                       $"\n\t\t- Seaweed: {seaweedLivingHere}" +
                       $"\n\t\t- Green Algae: {greenAlgaeLivingHere}" +
                       $"\n\t\t- White Algae: {whiteAlgaeLivingHere}";
        }

        var held = nearest.Read("ItemsHeld").ParseList<string>(";");
        if (held.Count > 0)
        {
            message += "\n\n\tAdditional items held:";
            foreach (var item in held.WhereNotNull())
            {
                var (index, stack, quality) = item.ParseTuple<int, int, Quality>()!.Value;
                var @object = new SObject(index, stack);
                message += $"\n\t\t- {@object.Name} x{stack} ({quality})";
            }
        }
        else
        {
            message += "\n\n\tThe pond holds no items.:";
        }

        var hasOrHasnt = nearest.Read<bool>("CheckedToday") ? "has" : "hasn't";
        message += $"\n\n\tThe pond {hasOrHasnt} been checked today.";
        Log.I(message);
    }
}