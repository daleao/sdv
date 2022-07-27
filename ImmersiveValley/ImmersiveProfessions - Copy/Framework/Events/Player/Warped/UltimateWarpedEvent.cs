namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Display;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation) || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

        if (e.NewLocation.IsDungeon())
        {
            Manager.Hook<UltimateMeterRenderingHudEvent>();
        }
        else
        {
            ModEntry.Player.RegisteredUltimate!.ChargeValue = 0.0;
            Manager.Unhook<UltimateMeterRenderingHudEvent>();
        }
    }
}