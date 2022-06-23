namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateInputUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.Config.SpecialActivationKey.IsDown()) ModEntry.PlayerState.RegisteredUltimate.UpdateInput();
    }
}