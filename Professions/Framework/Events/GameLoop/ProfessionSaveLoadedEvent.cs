namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Professions.Framework.Events.Display.RenderedHud;
using DaLion.Professions.Framework.Events.GameLoop.DayEnding;
using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.Input.ButtonsChanged;
using DaLion.Professions.Framework.Events.Multiplayer;
using DaLion.Professions.Framework.Events.Player;
using DaLion.Professions.Framework.Events.World.ObjectListChanged;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionSaveLoadedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSaveLoadedEvent(EventManager? manager = null)
    : SaveLoadedEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        this.Manager.Manage<ProfessionsChangedEvent>(player);

        ISkill.RevalidateAll();
        ISkill.CountResets();
        Profession.AddRecipes();
        Profession.EnableEvents();
        LimitBreak.Load();
    }
}
