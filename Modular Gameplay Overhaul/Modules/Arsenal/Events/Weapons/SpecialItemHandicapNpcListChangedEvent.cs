namespace DaLion.Overhaul.Modules.Arsenal.Events.Weapons;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class SpecialItemHandicapNpcListChangedEvent : NpcListChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="SpecialItemHandicapNpcListChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SpecialItemHandicapNpcListChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => ArsenalModule.Config.Weapons.EnableRebalance;

    /// <inheritdoc />
    protected override void OnNpcListChangedImpl(object? sender, NpcListChangedEventArgs e)
    {
        if (e.Location is not MineShaft)
        {
            return;
        }

        var monsters = e.Added.OfType<Monster>().ToArray();
        if (monsters.Any(m => m.hasSpecialItem.Value))
        {
            ArsenalModule.State.MonsterDropAccumulator = 0.0;
            return;
        }

        foreach (var monster in e.Added.OfType<Monster>())
        {
            if (Game1.random.NextDouble() < ArsenalModule.State.MonsterDropAccumulator)
            {
                monster.hasSpecialItem.Value = true;
                Game1.player.Write(DataFields.MonsterDropAccumulator, "0.0");
            }
            else
            {
                ArsenalModule.State.MonsterDropAccumulator += 0.00025;
            }
        }
    }
}
