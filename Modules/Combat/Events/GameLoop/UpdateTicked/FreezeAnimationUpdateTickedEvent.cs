namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class FreezeAnimationUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="FreezeAnimationUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal FreezeAnimationUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!FreezeAnimation.FreezeAnimationsByMonster.Any())
        {
            this.Disable();
        }

        FreezeAnimation.FreezeAnimationsByMonster.ForEach(pair => pair.Value.ForEach(freeze => freeze.update(Game1.currentGameTime)));
    }
}
