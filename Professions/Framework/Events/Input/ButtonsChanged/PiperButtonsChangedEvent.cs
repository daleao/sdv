namespace DaLion.Professions.Framework.Events.Input.ButtonsChanged;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperButtonsChangedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperButtonsChangedEvent(EventManager? manager = null)
    : ButtonsChangedEvent(manager ?? ProfessionsMod.EventManager)
{
    private static GreenSlime? _temporarilyPiped;

    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.ModKey.JustPressed() && _temporarilyPiped is null)
        {
            _temporarilyPiped = Game1.player.GetClosestCharacter<GreenSlime>(predicate: s => s.Get_Piped() is null);
            _temporarilyPiped?.Set_Piped(Game1.player, PipedSlime.PipingSource.Herded);
        }
        else if (Config.ModKey.GetState() == SButtonState.Released && _temporarilyPiped is not null)
        {
            _temporarilyPiped.Set_Piped(null);
            _temporarilyPiped = null;
        }
    }
}
