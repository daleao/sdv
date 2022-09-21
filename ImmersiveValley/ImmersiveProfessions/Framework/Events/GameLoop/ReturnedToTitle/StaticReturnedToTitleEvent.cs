namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <summary>Initializes a new instance of the <see cref="StaticReturnedToTitleEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal StaticReturnedToTitleEvent(ProfessionEventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Disable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object? sender, ReturnedToTitleEventArgs e)
    {
        // disable events
        this.Manager.ResetAllScreens();

        // reset mod state
        ModEntry.State = new ModState();
    }
}
