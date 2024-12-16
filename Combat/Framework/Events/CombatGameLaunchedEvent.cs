namespace DaLion.Combat.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="CombatGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class CombatGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? CombatMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        if (CombatConfigMenu.Instance?.IsLoaded == true)
        {
            CombatConfigMenu.Instance.Register();
        }
    }
}
