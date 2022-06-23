namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;
using StardewValley;

using Common.Commands;
using Common.Extensions;

#endregion using directives

internal class MaxFishingAuditCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "fishdex_complete";

    /// <inheritdoc />
    public string Documentation => "Set all fish to seen and caught at max-size.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsIn(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);
        foreach (var (key, value) in fishData)
        {
            var dataFields = value.Split('/');
            if (Game1.player.fishCaught.ContainsKey(key))
            {
                var caught = Game1.player.fishCaught[key];
                caught[1] = Convert.ToInt32(dataFields[4]) + 1;
                Game1.player.fishCaught[key] = caught;
                Game1.stats.checkForFishingAchievements();
            }
            else
            {
                Game1.player.fishCaught.Add(key, new[] { 1, Convert.ToInt32(dataFields[4]) + 1 });
            }
        }
    }
}