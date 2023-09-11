namespace DaLion.Overhaul.Modules.Combat.Events.Player;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SveWarpedEvent : WarpedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SveWarpedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SveWarpedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Name != "Custom_TreasureCave")
        {
            return;
        }

        //var obtained = e.Player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>();
        //if (obtained.Count >= 4)
        //{
        e.NewLocation.setTileProperty(10, 7, "Buildings", "Success", $"W {ItemIDs.LavaKatana} 1");
        //}
        //else
        //{
        //    var chosen = new[]
        //    {
        //        ItemIDs.GalaxySword,
        //        ItemIDs.GalaxyHammer,
        //        ItemIDs.GalaxyDagger,
        //        ItemIDs.GalaxySlingshot,
        //    }.Except(obtained).Choose();

        //    e.NewLocation.setTileProperty(10, 7, "Buildings", "Success", $"W {chosen} 1");
        //}

        e.Player.Write(DataKeys.ProvenValor, int.MaxValue.ToString());
        CombatModule.State.HeroQuest?.UpdateTrialProgress(Virtue.Valor);
    }
}
