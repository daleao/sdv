namespace DaLion.Overhaul.Modules.Professions.Events.Input;

#region using directives

using DaLion.Overhaul.Modules.Core.UI;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class TrackerButtonsChangedEvent : ButtonsChangedEvent
{
    /// <summary>Initializes a new instance of the <see cref="TrackerButtonsChangedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TrackerButtonsChangedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Prospector) ||
                                      Game1.player.HasProfession(Profession.Scavenger);

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (ProfessionsModule.Config.ModKey.JustPressed())
        {
            HudPointer.Instance.Value.ShouldBob = true;
        }
        else if (ProfessionsModule.Config.ModKey.GetState() == SButtonState.Released &&
                 !Game1.player.Get_ProspectorHunt().IsActive && !Game1.player.Get_ScavengerHunt().IsActive)
        {
            HudPointer.Instance.Value.ShouldBob = false;
        }
    }
}
