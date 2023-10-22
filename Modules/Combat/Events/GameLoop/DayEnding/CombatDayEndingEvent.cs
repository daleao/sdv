namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Monster_Bleeding.Values.Clear();
        Monster_Burnt.Values.Clear();
        Monster_Poisoned.Values.Clear();
        Monster_Slowed.Values.Clear();
        BleedAnimation.BleedAnimationByMonster.Clear();
        BurnAnimation.BurnAnimationsByMonster.Clear();
        PoisonAnimation.PoisonAnimationByMonster.Clear();
        SlowAnimation.SlowAnimationByMonster.Clear();
        StunAnimation.StunAnimationByMonster.Clear();
    }
}
