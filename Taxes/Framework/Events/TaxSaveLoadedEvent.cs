namespace DaLion.Taxes.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class TaxSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TaxSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxSaveLoadedEvent(EventManager? manager = null)
        : base(manager ?? TaxesMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        if (Data.ReadAs<int>(Game1.player, DataKeys.LatestAmountWithheld) > 0)
        {
            TaxesMod.EventManager.Enable<TaxDayStartedEvent>();
        }

        var farm = Game1.getFarm();
        if (!Game1.player.IsMainPlayer || Data.ReadAs(farm, DataKeys.UsableTiles, -1) > 0)
        {
            return;
        }

        var usableTiles = 0;
        for (var i = 0; i < farm.Map.DisplayHeight; i++)
        {
            for (var j = 0; j < farm.Map.DisplayWidth; j++)
            {
                if (farm.doesTileHaveProperty(i, j, "Diggable", "Back") is not null)
                {
                    usableTiles++;
                }
            }
        }

        Data.Write(farm, DataKeys.UsableTiles, usableTiles.ToString());
        Log.D($"Counted {usableTiles} usable tiles in {farm.Name}.");
    }
}
