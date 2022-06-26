namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticReturnedToTitleEvent(ProfessionEventManager manager)
        : base(manager)
    {
        AlwaysHooked = true;
    }

    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        // unhook events
        Manager.UnhookFromLocalPlayer();

        // reset mod state
        ModEntry.PlayerState = new();
    }
}