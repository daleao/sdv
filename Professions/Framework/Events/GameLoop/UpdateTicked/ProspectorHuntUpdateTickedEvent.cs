namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProspectorHuntUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProspectorHuntUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ProspectorHunt?.IsActive ?? false;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        State.ProspectorHunt!.TimeUpdate(e.Ticks);
        if (Game1.player.HasProfession(Profession.Prospector, true))
        {
            Game1.gameTimeInterval = 0;
        }

        if (State.ProspectorHunt.TreasureStone is not { } stone)
        {
            return;
        }

        var lightSource = Utility.getLightSource(stone.lightSource.Id);
        if (lightSource is null)
        {
            return;
        }

        var t = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 2d * Math.PI);
        var radius = MathHelper.Lerp(0.5f, 0.55f, t);
        lightSource.radius.Value = radius;
    }
}
