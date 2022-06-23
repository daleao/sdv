namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        ModEntry.PlayerState.RegisteredUltimate.CheckForActivation();
    }
}