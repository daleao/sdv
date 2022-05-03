namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class UltimateButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        ModEntry.PlayerState.RegisteredUltimate.CheckForActivation();
    }
}