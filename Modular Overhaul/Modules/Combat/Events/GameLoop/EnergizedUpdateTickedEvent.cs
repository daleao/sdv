namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private WeakReference<EnergizedEnchantment>? _energized;

    /// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var energized = (Game1.player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<EnergizedEnchantment>();
        if (energized is null)
        {
            this.Disable();
            return;
        }

        this._energized = new WeakReference<EnergizedEnchantment>(energized, false);
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._energized = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._energized is null || !this._energized.TryGetTarget(out var energized))
        {
            this.Disable();
            return;
        }

        energized.Update(e.Ticks);
    }
}
