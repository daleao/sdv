namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BurnAnimationRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="BurnAnimationRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BurnAnimationRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!BurnAnimation.BurnAnimationByMonster.Any())
        {
            this.Disable();
        }

        BurnAnimation.BurnAnimationByMonster.ForEach(pair => pair.Value.draw(e.SpriteBatch));
    }
}
