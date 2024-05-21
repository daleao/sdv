namespace DaLion.Core.Framework.Events;

#region using directives

using System.Linq;
using DaLion.Core.Framework.Debuffs;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="BleedAnimationUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class BleedAnimationUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? CoreMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!BleedAnimation.BleedAnimationByMonster.Any())
        {
            this.Disable();
        }

        BleedAnimation.BleedAnimationByMonster.ForEach(pair => pair.Value.Update(Game1.currentGameTime));
    }
}
