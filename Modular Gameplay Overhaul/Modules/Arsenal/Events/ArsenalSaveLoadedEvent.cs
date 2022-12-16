namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ArsenalSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ArsenalSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalSaveLoadedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        if (player.Read<bool>(DataFields.Cursed))
        {
            this.Manager.Enable<CurseUpdateTickedEvent>();
        }

        if (player.canUnderstandDwarves)
        {
            ModHelper.GameContent.InvalidateCache("Data/Events/Blacksmith");
        }

        if (player.Read(DataFields.DaysLeftTranslating, -1) > 0)
        {
            this.Manager.Enable<BlueprintDayStartedEvent>();
        }
    }
}
