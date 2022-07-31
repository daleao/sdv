namespace DaLion.Stardew.Ponds.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Enums;
using Common.Extensions.Stardew;
using Extensions;
using StardewValley.Buildings;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SetFishQualityCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetFishQualityCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_quality", "set", "quality" };

    /// <inheritdoc />
    public override string Documentation => "Set the quality of all fish in the nearest pond.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length != 1)
        {
            Log.W("You must specify a quality (`low`, `med`, `high` or `best`).");
            return;
        }

        if (!Game1.player.currentLocation.Equals(Game1.getFarm()))
        {
            Log.W("You must be at the farm to do this.");
            return;
        }

        var ponds = Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                !p.isUnderConstruction())
            .ToHashSet();
        if (ponds.Count <= 0)
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

        var newQuality = args[0] switch
        {
            "low" or "normal" or "regular" or "white" => Quality.Regular,
            "med" or "silver" => Quality.Silver,
            "high" or "gold" => Quality.Gold,
            "best" or "iridium" => Quality.Iridium,
            _ => (Quality)(-1)
        };

        if (newQuality < 0)
        {
            Log.W("Unexpected quality. Should be either low/regular, med/silver, high/gold or best/iridium.");
            return;
        }

        var familyCount = nearest.Read<int>("FamilyLivingHere");
        var familyQualities = new int[4];
        if (familyCount > nearest.FishCount)
        {
            Log.W("FamilyLivingHere data is invalid. The data will be reset.");
            familyCount = 0;
            nearest.Write("FamilyLivingHere", null);
        }

        if (familyCount > 0)
        {
            familyQualities[newQuality == Quality.Iridium ? 3 : (int)newQuality] += familyCount;
            nearest.Write("FamilyQualities", string.Join(',', familyQualities));
        }

        var fishQualities = new int[4];
        fishQualities[newQuality == Quality.Iridium ? 3 : (int)newQuality] += nearest.FishCount - familyCount;
        nearest.Write("FishQualities", string.Join(',', fishQualities));
    }
}