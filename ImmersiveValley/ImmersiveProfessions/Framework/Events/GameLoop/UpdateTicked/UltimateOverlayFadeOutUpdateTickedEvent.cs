namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateOverlayFadeOutUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateOverlayFadeOutUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        ModEntry.PlayerState.RegisteredUltimate!.Overlay.FadeOut();
    }
}