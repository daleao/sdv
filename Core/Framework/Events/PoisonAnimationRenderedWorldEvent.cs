namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Core.Framework.Debuffs;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PoisonAnimationRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PoisonAnimationRenderedWorldEvent(EventManager? manager = null)
    : RenderedWorldEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!PoisonAnimation.PoisonAnimationByMonster.Any())
        {
            this.Disable();
        }

        PoisonAnimation.PoisonAnimationByMonster.ForEach(pair => pair.Value.draw(e.SpriteBatch));
    }
}
