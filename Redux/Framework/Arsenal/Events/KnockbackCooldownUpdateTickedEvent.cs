namespace DaLion.Redux.Framework.Arsenal.Events;

#region using directives

using DaLion.Redux.Framework.Arsenal.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class KnockbackCooldownUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="KnockbackCooldownUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal KnockbackCooldownUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Arsenal.KnockbackImmuneMonsters.Count == 0)
        {
            this.Disable();
            return;
        }

        foreach (var monster in ModEntry.State.Arsenal.KnockbackImmuneMonsters)
        {
            monster.CountdownKnockbackCooldown();
        }
    }
}
