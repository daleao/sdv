namespace DaLion.Stardew.FishPonds;

#region using directives

using System.Linq;
using StardewValley;
using StardewValley.Buildings;
using StardewModdingAPI;

using Common.Extensions;
using Framework.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

internal static class ConsoleCommands
{
    internal static void Register(ICommandHelper helper)
    {
        helper.Add("aqua_maxquality", "Set the quality of all fish in the nearest pond to the local player.",
            SetNearestQualities);
    }

    #region command handlers

    /// <summary>Set the quality rating of the nearest pond such that all fish </summary>
    /// <param name="command"></param>
    /// <param name="args"></param>
    internal static void SetNearestQualities(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        if (args.Length != 1)
        {
            Log.W("You must specify a quality. Quality should be one of <low, med, high, best>");
            return;
        }

        if (!args[0].IsAnyOf("low", "med", "high", "best"))
        {
            Log.W("Quality should be one of <low, med, high, best>");
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
        if (!ponds.Any())
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

#pragma warning disable CS8509
        var newQuality = args[0] switch
#pragma warning restore CS8509
        {
            "low" or "normal" or "regular" or "white" => SObject.lowQuality,
            "med" or "silver" => SObject.medQuality,
            "high" or "gold" => SObject.highQuality,
            "best" or "iridium" => SObject.bestQuality
        };

        var familyCount = nearest.ReadDataAs<int>("FamilyLivingHere");
        var familyQualities = new int[4];
        if (familyCount > nearest.FishCount)
        {
            Log.W("FamilyLivingHere data is invalid. The data will be reset.");
            familyCount = 0;
            nearest.WriteData("FamilyLivingHere", null);
        }

        if (familyCount > 0)
        {
            familyQualities[newQuality == 4 ? 3 : newQuality] += familyCount;
            nearest.WriteData("FamilyQualities", string.Join(',', familyQualities));
        }

        var fishQualities = new int[4];
        fishQualities[newQuality == 4 ? 3 : newQuality] += nearest.FishCount - familyCount;
        nearest.WriteData("FishQualities", string.Join(',', fishQualities));
    }

    #endregion command handlers
}