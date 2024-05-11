namespace DaLion.Professions.Framework.Events.World.BuildingListChanged;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PrestigedPiperBuildingListChangedEvent : BuildingListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="PrestigedPiperBuildingListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PrestigedPiperBuildingListChangedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper, true, true);

    /// <inheritdoc />
    protected override void OnBuildingListChangedImpl(object? sender, BuildingListChangedEventArgs e)
    {
        foreach (var building in e.Added)
        {
            if (building.indoors.Value is SlimeHutch hutch)
            {
                Reflector
                    .GetUnboundFieldSetter<SlimeHutch, int>(hutch, "_slimeCapacity")
                    .Invoke(hutch, 30);
                hutch.waterSpots.SetCount(6);
            }
        }
    }
}
