namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common.Commands;
using Common.Extensions;
using Framework;
using JetBrains.Annotations;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxFishingAuditCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxFishingAuditCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "fishdex_complete", "set_fishdex" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Set all fish to seen and caught at max-size. Relevant for {Profession.Angler}s.";

    /// <inheritdoc />
    public override void Callback(string[] args)
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