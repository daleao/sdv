namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Enchantments.Framework.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ExplosiveUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ExplosiveUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? EnchantmentsMod.EventManager)
{
    private WeakReference<ExplosiveEnchantment>? _explosive;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var explosive = (Game1.player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<ExplosiveEnchantment>();
        if (explosive is null)
        {
            this.Disable();
            return;
        }

        this._explosive = new WeakReference<ExplosiveEnchantment>(explosive, false);
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        this._explosive = null;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (this._explosive is null || !this._explosive.TryGetTarget(out var explosive))
        {
            this.Disable();
            return;
        }

        explosive.Update();
    }
}
