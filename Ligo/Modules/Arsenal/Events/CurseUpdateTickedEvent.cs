namespace DaLion.Ligo.Modules.Arsenal.Events;

#region using directives

using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class CurseUpdateTickedEvent : UpdateTickedEvent
{
    private MeleeWeapon? _cursedWeapon;

    /// <summary>Initializes a new instance of the <see cref="CurseUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CurseUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._cursedWeapon = Game1.player.Items.FirstOrDefault(i => i is MeleeWeapon
        {
            InitialParentTileIndex: Constants.DarkSwordIndex
        }) as MeleeWeapon;

        if (this._cursedWeapon is not null)
        {
            return;
        }

        Log.W($"Cursed farmer {Game1.player.Name} is not carrying the Dark Sword.");
        this.Disable();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        if (player.CurrentTool != this._cursedWeapon ||
            (!Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus) || !Game1.shouldTimePass() ||
            !e.IsMultipleOf(300))
        {
            return;
        }

        var dot = (this._cursedWeapon!.Read<int>(DataFields.CursePoints) - 400) / 50;
        player.health = Math.Max(player.health - dot, 1);
    }
}
