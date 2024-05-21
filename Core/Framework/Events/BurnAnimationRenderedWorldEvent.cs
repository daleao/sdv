namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Core.Framework.Debuffs;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="BurnAnimationRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class BurnAnimationRenderedWorldEvent(EventManager? manager = null)
    : RenderedWorldEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!BurnAnimation.BurnAnimationsByMonster.Any())
        {
            this.Disable();
        }

        BurnAnimation.BurnAnimationsByMonster.ForEach(pair => pair.Value.ForEach(burn => burn.draw(e.SpriteBatch)));
    }
}
