namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxFishingAuditCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="MaxFishingAuditCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxFishingAuditCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "fishdex_complete", "set_fishdex" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Set all fish to seen and caught at max-size. Relevant for {Profession.Angler}s.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        foreach (var (key, value) in ModHelper.GameContent.Load<Dictionary<int, string>>("Data/Fish"))
        {
            if (key is 152 or 153 or 157 || value.Contains("trap"))
            {
                continue;
            }

            var dataFields = value.SplitWithoutAllocation('/');
            if (Game1.player.fishCaught.ContainsKey(key))
            {
                var caught = Game1.player.fishCaught[key];
                caught[1] = int.Parse(dataFields[4]) + 1;
                Game1.player.fishCaught[key] = caught;
                Game1.stats.checkForFishingAchievements();
            }
            else
            {
                Game1.player.fishCaught.Add(key, new[] { 1, int.Parse(dataFields[4]) + 1 });
            }
        }
    }
}
