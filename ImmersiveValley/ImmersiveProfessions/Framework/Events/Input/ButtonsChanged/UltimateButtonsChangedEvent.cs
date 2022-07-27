namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, UltimateEvent]
internal sealed class UltimateButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateButtonsChangedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        Game1.player.get_Ultimate()!.CheckForActivation();
    }
}