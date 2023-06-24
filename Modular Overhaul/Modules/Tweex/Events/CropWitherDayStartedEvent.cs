namespace DaLion.Overhaul.Modules.Tweex.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class CropWitherDayStartedEvent : DayStartedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CropWitherDayStartedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CropWitherDayStartedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer && TweexModule.Config.CropWitherChance > 0f;

    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        for (var i = 0; i < Game1.locations.Count; i++)
        {
            var location = Game1.locations[i];
            foreach (var dirt in location.terrainFeatures.Values.OfType<HoeDirt>())
            {
                if (dirt.crop is null || dirt.crop.dead.Value || dirt.state.Value == 1)
                {
                    dirt.Write(DataKeys.DaySinceWatered, null);
                }
                else if (dirt.crop is { dead.Value: false, forageCrop.Value: false } crop && dirt.state.Value == 0)
                {
                    if (crop.indexOfHarvest.Value == ItemIDs.Fiber)
                    {
                        continue;
                    }

                    var daysSinceWatered = dirt.Read<int>(DataKeys.DaySinceWatered);
                    if (Game1.random.NextDouble() < TweexModule.Config.CropWitherChance * (daysSinceWatered - 1))
                    {
                        dirt.crop.Kill();
                    }
                    else
                    {
                        dirt.Increment(DataKeys.DaySinceWatered);
                    }
                }
            }
        }
    }
}
