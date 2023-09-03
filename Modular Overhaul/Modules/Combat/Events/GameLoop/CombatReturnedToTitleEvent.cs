namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatReturnedToTitleEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatReturnedToTitleEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        BleedAnimation.BleedAnimationByMonster.Clear();
        BurnAnimation.BurnAnimationByMonster.Clear();
        PoisonAnimation.PoisonAnimationByMonster.Clear();
        SlowAnimation.SlowAnimationByMonster.Clear();
        StunAnimation.StunAnimationByMonster.Clear();
    }
}
