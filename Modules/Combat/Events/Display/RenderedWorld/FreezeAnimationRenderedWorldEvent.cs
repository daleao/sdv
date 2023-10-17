namespace DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class FreezeAnimationRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="FreezeAnimationRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal FreezeAnimationRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!FreezeAnimation.FreezeAnimationsByMonster.Any())
        {
            this.Disable();
        }

        FreezeAnimation.FreezeAnimationsByMonster.ForEach(pair => pair.Value.ForEach(freeze => freeze.draw(e.SpriteBatch)));
    }
}
